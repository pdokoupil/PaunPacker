using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using PaunPacker.Core.Packing.MBBF;
using PaunPacker.Core.Packing.Placement;
using PaunPacker.Core.Packing.Placement.MaximalRectangles;
using PaunPacker.Core.Packing.Placement.Skyline;
using PaunPacker.Core.Packing.Sorting;
using PaunPacker.Tests.Benchmarks.Support;
using Unity;

namespace PaunPacker.Tests.Benchmarks
{
    /// <summary>
    /// This class uses BenchmarkDotNet libarary to run benchmarks
    /// </summary>
    class BenchmarkRunner
    {
        /// <summary>
        /// "Show help" command
        /// </summary>
        private const char help = 'h';

        /// <summary>
        /// "Quit benchmark program" command
        /// </summary>
        private const char quit = 'q';

        /// <summary>
        /// "Clear console output" command
        /// </summary>
        private const char clear = 'c';

        /// <summary>
        /// "Run benchmarks of placement algorithms" command
        /// </summary>
        private const char placementBenchmark = 'p';
        
        /// <summary>
        /// "Run benchmarks of minimum bounding box finders" command
        /// </summary>
        private const char minimumBoundingBoxFinderBenchmark = 'm';

        /// <summary>
        /// Minimum bounding box finders that were imported from the plugins
        /// </summary>
        private static IEnumerable<Type> MinimumBoundingBoxFinderTypes { get; set; }

        /// <summary>
        /// Placement algorithms that were imported from the plugins
        /// </summary>
        private static IEnumerable<Type> PlacementAlgorithmTypes { get; set; }

        /// <summary>
        /// Benchmark program entrypoint.
        /// Accepts and then performs commands from the user
        /// </summary>
        [STAThread] //This thread has to be STA because Prism is doing something with windows (probably)
        static void Main()
        {
            PluginTypeLoader.LoadTypes();
            MinimumBoundingBoxFinderTypes = PluginTypeLoader.MinimumBoundingBoxFinderTypes;
            PlacementAlgorithmTypes = PluginTypeLoader.PlacementAlgorithmTypes;
            
            char c = '\0';
            while (true)
            {
                if (c != help)
                {
                    ShowHelp();
                }
                var str = Console.ReadLine();

                while (str.Length != 1)
                {
                    Console.WriteLine("\n" + Separator);
                    Console.WriteLine("Error, uknown command, showing help:");
                    ShowHelp();
                    str = Console.ReadLine();
                }

                c = str[0];

                switch (c)
                {
                    case help:
                        ShowHelp();
                        break;
                    case quit:
                        Console.WriteLine("Good bye");
                        return;
                    case clear:
                        Console.Clear();
                        ShowHelp();
                        break;
                    case placementBenchmark:
                        RunTests<IPlacementAlgorithm>();
                        break;
                    case minimumBoundingBoxFinderBenchmark:
                        RunTests<IMinimumBoundingBoxFinder>();
                        break;
                    default:
                        Console.WriteLine("\n" + Separator);
                        Console.WriteLine("Error, uknown command, showing help:");
                        ShowHelp(); 
                        break;
                }
            }
        }
        
        /// <summary>
        /// Shows a help for the bechmark program
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine("PaunPacker Benchmarks");
            Console.WriteLine(Separator);
            Console.WriteLine("Commands that could be entered (without quotes) and confirmed by pressing ENTER:");
            Console.WriteLine("\tType \'m\' to run MinimumBoundingBoxFinder benchmarks");
            Console.WriteLine("\tType \'p\' to run PlacementAlgorithm benchmarks");
            Console.WriteLine("\tType \'q\' to quit the program");
            Console.WriteLine("\tType \'h\' to display help");
            Console.WriteLine("\tType \'c\' to clear the output");
            Console.WriteLine(Separator);
            Console.WriteLine(Separator);
        }

        /// <summary>
        /// Accepts additional parameters and runs the tests inside the given class
        /// </summary>
        /// <typeparam name="T">The type of class that contains the tests</typeparam>
        private static void RunTests<T>()
        {
            if (typeof(T) != typeof(IPlacementAlgorithm) &&
                typeof(T) != typeof(IMinimumBoundingBoxFinder))
            {
                throw new NotSupportedException();
            }

            string str = "";

            IImageSorter selectedImageSorter = null;
            IPlacementAlgorithm selectedPlacementAlgorithm = null;

            //Ask the user to select an image sorter that will parametrize the placement algorithm / minimum bounding box finder
            while (true)
            {
                do
                {
                    Console.WriteLine("Select image sorter to be used: ");
                    Console.WriteLine($"\tEnter 'a' to select {typeof(ByHeightAndWidthImageSorter).Name}");
                    Console.WriteLine($"\tEnter 'b' to select {typeof(ByHeightAndWidthImageSorterDesc).Name}");
                    Console.WriteLine($"\tEnter 'c' to select {typeof(PreserveOrderImageSorter).Name}");
                } while ((str = Console.ReadLine()).Length != 1);

                switch (str[0])
                {
                    case 'a':
                        selectedImageSorter = new ByHeightAndWidthImageSorter();
                        break;
                    case 'b':
                        selectedImageSorter = new ByHeightAndWidthImageSorterDesc();
                        break;
                    case 'c':
                        selectedImageSorter = new PreserveOrderImageSorter();
                        break;
                    default:
                        continue;
                }

                break;
            }

            //If the users wants to benchmark Minimum bounding box finders
            //  then ask the user to select an placement algorithm that will parametrize tminimum bounding box finders
            if (typeof(T) == typeof(IMinimumBoundingBoxFinder))
            {
                while (true)
                {
                    do
                    {
                        Console.WriteLine("Select placement algorithm to be used: ");
                        Console.WriteLine($"\tEnter 'a' to select {typeof(BLAlgorithmPacker).Name}");
                        Console.WriteLine($"\tEnter 'b' to select {typeof(SkylineAlgorithm).Name}");
                        Console.WriteLine($"\tEnter 'c' to select {typeof(MaximalRectanglesAlgorithm).Name}");
                    } while ((str = Console.ReadLine()).Length != 1);

                    switch (str[0])
                    {
                        case 'a':
                            selectedPlacementAlgorithm = new BLAlgorithmPacker();
                            break;
                        case 'b':
                            selectedPlacementAlgorithm = new SkylineAlgorithm();
                            break;
                        case 'c':
                            selectedPlacementAlgorithm = new MaximalRectanglesAlgorithm();
                            break;
                        default:
                            continue;
                    }

                    break;
                }
            }

            bool isItSquaresTest = true;
            int numOfRects = 0;
            int seed = 0;

            //Ask the user to select test type
            //Currently, two types of tests are implemented (differing in the input sequence)
            // 1) Input sequence consisting of n squares with sizes 1x1, ..., nxn (given in random order)
            // 2) Input sequence consisting of n rectangles with random (but upper-bounded) sizes (given in random order)
            while (true)
            {
                str = "";
                do
                {
                    Console.WriteLine("Select test type:");
                    Console.WriteLine("\tEnter 'a' to perform test with squares of sizes 1x1, 2x2, ..., nxn");
                    Console.WriteLine("\tEnter 'b' to perform test with n rectangles of random sizes");
                } while ((str = Console.ReadLine()).Length != 1);

                switch (str[0])
                {
                    case 'b':
                    case 'a':

                        isItSquaresTest = str[0] == 'a';

                        //Ask the user to enter number of rectangles/squares
                        while (true)
                        {
                            Console.WriteLine("\tEnter number of 'n' - rectangles(squares)");
                            str = Console.ReadLine();
                            if (Int32.TryParse(str, out numOfRects))
                            {
                                if (numOfRects > 0)
                                {
                                    break;
                                }
                            }
                        }

                        //If it is the random rectangles test then ask the user to enter the random seed
                        //  The seed will be user to construct the random generator used to generate the random rectangle sizes
                        if (!isItSquaresTest)
                        {
                            while (true)
                            {
                                Console.WriteLine("\tEnter seed");
                                str = Console.ReadLine();
                                if (Int32.TryParse(str, out seed))
                                {
                                    break;
                                }
                            }
                        }

                        break;
                    default:
                        continue;
                }
                break;
            }

            //Now when the user has selected the type of test and all the parameters
            // It is time to actaully run the tests. But because we need to test types
            // that are contained within plugins and also because BenchmarkDotNet (in a standard scenario)
            // requires to use (static) attribute [Benchmark] on the methods that should be called
            // we have decided to generate the benchmark class (with benchmark methods) dynamically at runtime
            // and then compile the generated class and pass the type of this class to the BenchmarkRunner
            
            string assemblyName = Path.GetRandomFileName();

            //Assemblies referenced by the assembly that will be generated at runtime
            //For each type that will be used in the generated test, an assembly containing
            //the used type has to be loaded
            var references = new List<MetadataReference>()
            {
                MetadataReference.CreateFromFile(typeof(BenchmarkDotNet.Attributes.BenchmarkAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.CSharp")).Location),
                MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("netstandard")).Location),
                MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("mscorlib")).Location),
                MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location),
                MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Unity.Container")).Location),
                MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Unity.Abstractions")).Location),
                MetadataReference.CreateFromFile(typeof(IMinimumBoundingBoxFinder).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(TestUtil).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(CancellationToken).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(UnityContainerExtensions).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Random).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Collections.Concurrent.ConcurrentDictionary<string, int>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ValueTuple<long, long>).Assembly.Location)
            };

            //Create same IoC here and in the generated class
            //This is because inside this class we try if (using the same IoC container with same registrations)
            //a given type could be resolved and if it does not, we exclude benchmark for this type in the generated class
            using UnityContainer IoC = new UnityContainer();
            IoC.RegisterFactory<IImageSorter>(_ => selectedImageSorter);

            if (selectedPlacementAlgorithm != null)
            {
                IoC.RegisterFactory<IPlacementAlgorithm>(_ => selectedPlacementAlgorithm);
            }
            
            //The generated source
            var benchmarkClassTemplate = new StringBuilder();
            //Using statements
            benchmarkClassTemplate.AppendLine("using System;");
            benchmarkClassTemplate.AppendLine("using Unity;");
            benchmarkClassTemplate.AppendLine("using System.Linq;");
            benchmarkClassTemplate.AppendLine("using PaunPacker.Core.Packing.MBBF;");
            benchmarkClassTemplate.AppendLine("using PaunPacker.Core.Packing.Placement;");
            benchmarkClassTemplate.AppendLine("using PaunPacker.Core.Packing.Sorting;");
            benchmarkClassTemplate.AppendLine("using PaunPacker.Core.Packing.Placement.Skyline;");
            benchmarkClassTemplate.AppendLine("using PaunPacker.Core.Packing.Placement.Guillotine;");
            benchmarkClassTemplate.AppendLine("using PaunPacker.Core.Packing.Placement.MaximalRectangles;");
            benchmarkClassTemplate.AppendLine("using System.Collections.Generic;");
            benchmarkClassTemplate.AppendLine("using System.Threading;");
            benchmarkClassTemplate.AppendLine("using PaunPacker.Tests;");

            //Beginning of the class
            benchmarkClassTemplate.AppendLine($"public class {typeof(T).Name}Benchmarks");
            benchmarkClassTemplate.AppendLine("{");
            //Member declarations
            benchmarkClassTemplate.AppendLine(@"
                private static UnityContainer IoC { get; set; }
                private static Random rnd;
                private static List<PaunPacker.Core.Types.PPRect> randomRects;
                static System.Collections.Concurrent.ConcurrentDictionary<string, ValueTuple<long, long>> results;
");
            benchmarkClassTemplate.AppendLine($"\n\tstatic {typeof(T).Name}Benchmarks()");
            benchmarkClassTemplate.AppendLine("\t{");

            //Constructor code
            benchmarkClassTemplate.AppendLine($@"
                IoC = new UnityContainer();
                IoC.RegisterInstance<IImageSorter>(new {selectedImageSorter.GetType().Name}());

                results = new System.Collections.Concurrent.ConcurrentDictionary<string, ValueTuple<long, long>>();

                rnd = new Random(" + seed + @");
                randomRects = new List<PaunPacker.Core.Types.PPRect>(" + numOfRects + @");
                
                int dimensionBound = Int32.MaxValue / " + numOfRects + @";
                dimensionBound = dimensionBound > 2049 ? 2049 : dimensionBound;

                for (int i = 0; i < " + numOfRects + @"; i++)
                {
                    randomRects.Add(new PaunPacker.Core.Types.PPRect(0, 0, rnd.Next(1, dimensionBound), rnd.Next(1, dimensionBound)));
                }
");

            if (selectedPlacementAlgorithm != null)
            {
                benchmarkClassTemplate.AppendLine($"IoC.RegisterInstance<IPlacementAlgorithm>(new { selectedPlacementAlgorithm.GetType().Name }());");
            }
            
            benchmarkClassTemplate.AppendLine("\t}");
            //Constructor ends here

            if (isItSquaresTest)
            {
                benchmarkClassTemplate.AppendLine($@"
                [BenchmarkDotNet.Attributes.ParamsSourceAttribute(nameof(ValuesForN))]
                public int N;
                public IEnumerable<int> ValuesForN => System.Linq.Enumerable.Range(1, {numOfRects});
");
            }

            int numOfTests = 0;
            // We are testing minimum bounding box finders, so generate tests for them
            if (selectedPlacementAlgorithm != null)
            {
                foreach (var x in MinimumBoundingBoxFinderTypes)
                {
                    try
                    {
                        UnityContainerExtensions.Resolve(IoC, Type.GetType(x.AssemblyQualifiedName), null);
                        references.Add(MetadataReference.CreateFromFile(x.Assembly.Location));
                        benchmarkClassTemplate.AppendLine("\t[BenchmarkDotNet.Attributes.BenchmarkAttribute]");
                        benchmarkClassTemplate.AppendLine($"\tpublic void Test{x.Name}()");
                        benchmarkClassTemplate.AppendLine("{");
                        benchmarkClassTemplate.AppendLine($"\tvar x = Type.GetType(\"{x.AssemblyQualifiedName}\");");
                        benchmarkClassTemplate.AppendLine($"\tvar res = (UnityContainerExtensions.Resolve(IoC, x, null) as IMinimumBoundingBoxFinder).FindMinimumBoundingBox(TestUtil.Shuffle(" + (isItSquaresTest ? "TestUtil.GetIncreasingSquares(N)" : $"randomRects") + "), CancellationToken.None);");
                        benchmarkClassTemplate.AppendLine($"results.AddOrUpdate(\"Test{x.Name}\", (res.Width * res.Height, 1), (key, old) => ((long)(old.Item1 + (double)(res.Width * res.Height - old.Item1) / (double)(old.Item2 + 1)), old.Item2 + 1));");
                        benchmarkClassTemplate.AppendLine("}");
                        benchmarkClassTemplate.AppendLine();
                        numOfTests++;
                    }
                    catch (ResolutionFailedException)
                    {
                        //Do not add benchmark for types that could not be resolved (for example types that are extensible by other plugins
                        // via plugin view ...). These types will simply not be benchmarked.
                    }
                }
            }
            else
            {
                foreach (var x in PlacementAlgorithmTypes)
                {
                    try
                    {
                        UnityContainerExtensions.Resolve(IoC, Type.GetType(x.AssemblyQualifiedName), null);
                        references.Add(MetadataReference.CreateFromFile(x.Assembly.Location));
                        benchmarkClassTemplate.AppendLine("\t[BenchmarkDotNet.Attributes.BenchmarkAttribute]");
                        benchmarkClassTemplate.AppendLine($"\tpublic void Test{x.Name}()");
                        benchmarkClassTemplate.AppendLine("{");
                        benchmarkClassTemplate.AppendLine($"\tvar x = Type.GetType(\"{x.AssemblyQualifiedName}\");");
                        benchmarkClassTemplate.AppendLine($"\tvar res = (UnityContainerExtensions.Resolve(IoC, x, null) as IPlacementAlgorithm).PlaceRects(Int32.MaxValue, Int32.MaxValue, TestUtil.Shuffle(" + (isItSquaresTest ? "TestUtil.GetIncreasingSquares(N)" : $"randomRects") + "));");
                        benchmarkClassTemplate.AppendLine("long actualW = res.Rects.Max(y => y.Right); long actualH = res.Rects.Max(y => y.Bottom);");
                        benchmarkClassTemplate.AppendLine($"checked {{ results.AddOrUpdate(\"Test{x.Name}\", (actualW * actualH, 1), (key, old) => ((long)(old.Item1 + (double)(actualW * actualH - old.Item1) / (double)(old.Item2 + 1)), old.Item2 + 1));");
                        benchmarkClassTemplate.AppendLine("}}");
                        benchmarkClassTemplate.AppendLine();
                        numOfTests++;
                    }
                    catch (ResolutionFailedException)
                    {
                        //Do not add benchmark for types that could not be resolved (for example types that are extensible by other plugins
                        // via plugin view ...). These types will simply not be benchmarked.
                    }
                }
            }

            //Global cleanup method
            //Because BenchmarkDotNet does not allow to report benchmark methods return values
            //We had to "hack" around it by using these two methods
            //And save the method results inside a file and then (when showing the report) load it from the file
            //The GlobalCleanup should not be measured as a part of the benchmark
            //However we still need to somewhere remember the return value of the methods
            //Because writing to file in the benchmark method itself would totally kill the performance, we have decided to store it in dictionary
            //Which still causes some test distortion but it should be OK (all the tests use it so their base-line is just shifted)
            benchmarkClassTemplate.AppendLine(@"

            [BenchmarkDotNet.Attributes.GlobalCleanup]
            public void GlobalCleanup()
            {
                //We want to call it only once, not after every [Benchmark] method
                if (results.Count() == " + numOfTests + @")
                {
                    using (var sw = new System.IO.StreamWriter(""results.txt"", true))
                    {
                        //foreach (var r in randomRects) //for debug purposes
                        //    sw.WriteLine(r.Width + ""x"" + r.Height);
                        foreach (var res in results)
                            sw.WriteLine(res.Key + "";"" + res.Value.Item1 + "";"" + res.Value.Item1 + "";"" + res.Value.Item2);
                    }
                    results.Clear();
                }
            }
");

            benchmarkClassTemplate.AppendLine("}");
            //Benchmark class ends here

            Console.WriteLine("Please wait ... (Compiling the Benchmarks)");

            //Create syntax tree and compile it
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(benchmarkClassTemplate.ToString());

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

            //Emit the CIL into memory stream
            using var ms = new MemoryStream();
            EmitResult result = compilation.Emit(ms);
            if (!result.Success)
            {
                IEnumerable<Microsoft.CodeAnalysis.Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (Microsoft.CodeAnalysis.Diagnostic diagnostic in failures)
                {
                    Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                }
            }
            else
            {
                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());
                Type type = assembly.GetType($"{typeof(T).Name}Benchmarks");
                Console.WriteLine("Starting the Benchmarks");
                var x = BenchmarkDotNet.Running.BenchmarkRunner.Run(type, new Config());
                Console.WriteLine("Done");
            }
        }

        /// <summary>
        /// Configuration for the benchmark
        /// </summary>
        private class Config : BenchmarkDotNet.Configs.ManualConfig
        {
            public Config()
            {
                //Our configuration is based on the DebugInProcessConfig, but adds one more column to it
                var x = new BenchmarkDotNet.Configs.DebugInProcessConfig();

                //So basically "Copy" the DebugInProcessConfig
                Options = BenchmarkDotNet.Configs.ConfigOptions.DisableOptimizationsValidator | BenchmarkDotNet.Configs.ConfigOptions.KeepBenchmarkFiles;
                Orderer = x.Orderer;
                SummaryStyle = x.SummaryStyle;
                ArtifactsPath = x.ArtifactsPath;

                if (BenchmarkDotNet.Jobs.Job.Default == x.GetJobs().First())
                {
                    Console.WriteLine();
                }

                Add(x.GetJobs().First());
                Add(x.GetLoggers().First());

                foreach (var colProvider in x.GetColumnProviders())
                {
                    Add(colProvider);
                }

                //And add one column for mean area of the packing result, because only measuring time would be meaningless for packing ...
                Add(new BenchmarkDotNet.Columns.TagColumn("Mean area", (k) =>
                {
                    //Benchmarks just finished
                    //So read the file and obtain the mean return values of the benchmark methods
                    if (results == null)
                    {
                        //Results are stored in form: Name of the test method : Queue of average packing result areas
                        //The reason for stack is that a method with same methodname could by called for different values of N
                        //In the output these are sorted increasingly so it is safe to do it via Queue
                        results = new Dictionary<string, Queue<string>>();
                        using (var sr = new StreamReader("results.txt"))
                        {
                            string line = "";
                            while ((line = sr.ReadLine()) != null)
                            {
                                var chunks = line.Split(";");
                                if (chunks.Length >= 2)
                                {
                                    if (!results.ContainsKey(chunks[0]))
                                    {
                                        results[chunks[0]] = new Queue<string>();
                                    }
                                    results[chunks[0]].Enqueue(chunks[1]);
                                }
                                //otherwise Skip
                            }
                        }
                        //Delete the file with results
                        File.Delete("results.txt");
                    }
                    return results[k].Dequeue();
                }));
            }

            private Dictionary<string, Queue<string>> results;
        }

        /// <summary>
        /// Used as a separator of lines in the console output
        /// </summary>
        private static string Separator => new string('=', Console.WindowWidth);


    }
}
