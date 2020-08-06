using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using PaunPacker.Core.Packing.MBBF;
using PaunPacker.Core.Packing.Placement;
using PaunPacker.GUI.WPF.Common.Attributes;
using Prism.Ioc;
using Prism.Modularity;
using Unity;

//This class should be possibly refactored in the future in order to decrease amount of code duplicity
namespace PaunPacker.Tests.Benchmarks.Support
{
    /// <summary>
    /// This class represents a wrapper around <see cref="Prism.Unity.PrismApplication"/>
    /// And it servers purely as a helper class for the BenchmarkRunner project
    /// The main purpose of this class is to provide ability to load types/instances of certain extensible
    /// components so that they could later be used to dynamically generate benchmarks.
    /// </summary>
    public static class PluginTypeLoader
    {
        /// <summary>
        /// Loaded MinimumBoundingBoxFinders
        /// </summary>
        public static IEnumerable<Type> MinimumBoundingBoxFinderTypes { get; private set; }

        /// <summary>
        /// Loaded PlacementAlgorithms
        /// </summary>
        public static IEnumerable<Type> PlacementAlgorithmTypes { get; private set; }

        /// <summary>
        /// Runs the <see cref="App"/> which loads the plugins and imports the types from them
        /// </summary>
        /// <param name="args"></param>
        public static void LoadTypes()
        {
            App app = new App();
            app.Initialize();

            MinimumBoundingBoxFinderTypes = app.MinimumBoundingBoxFinderTypes;
            PlacementAlgorithmTypes = app.PlacementAlgorithmTypes;

            app.Shutdown();
            app.Dispose();
        }

        /// <summary>
        /// Easiest way to load the plugins and import their types is to use <see cref="Prism.Unity.PrismApplication"/>
        /// This is (simplified) version of PaunPacker.GUI.App class
        /// </summary>
        private sealed class App : Prism.Unity.PrismApplication, IDisposable
        {
            protected override System.Windows.Window CreateShell()
            {
                return null;
            }

            protected override void RegisterTypes(IContainerRegistry containerRegistry)
            {

            }

            //Based on https://github.com/PrismLibrary/Prism/issues/1732
            /// <summary>
            /// Creates the module catalog from and loads the plugins from a plugin folder
            /// It also loads the plugins using the MEF
            /// </summary>
            /// <remarks>This is also somehow a workaround because Prism's DirectoryModule is not supported on .NET Core
            /// so the plugins from a folder has to be loaded manually</remarks>
            protected override IModuleCatalog CreateModuleCatalog()
            {
                AssemblyLoadContext ctx = AssemblyLoadContext.GetLoadContext(this.GetType().Assembly);

                DirectoryInfo directory;
                IEnumerable<FileInfo> fileInfos;

                try
                {
                    directory = new DirectoryInfo("plugins");
                    var alreadyLoadedAssemblies = ctx.Assemblies;
                    fileInfos = directory.GetFiles("*.dll")
                        .Where(file => alreadyLoadedAssemblies
                        .FirstOrDefault(assembly => String.Compare(Path.GetFileName(assembly.Location), file.Name,
                                                                        StringComparison.OrdinalIgnoreCase) == 0) == null);
                }
                catch (DirectoryNotFoundException)
                {
                    //Simply skip the plugin loading and return empty module catalog
                    fileInfos = Enumerable.Empty<FileInfo>();
                    Console.WriteLine("Plugins not found !");
                }

                var mefConfiguration = new ContainerConfiguration();

                List<Assembly> validAssemblies = new List<Assembly>();
                foreach (FileInfo fileInfo in fileInfos)
                {
                    try
                    {
                        var assembly = ctx.LoadFromAssemblyPath(fileInfo.FullName);
                        mefConfiguration.WithAssembly(assembly);
                        validAssemblies.Add(assembly);
                    }
                    catch (BadImageFormatException)
                    {
                        // skip non-.NET Dlls
                    }
                    catch (ReflectionTypeLoadException)
                    {

                    }
                    catch (FileLoadException)
                    {
                    }
                }

                //Although this could be done, it is probably better not to polute PaunPacker.Core with [Export] attributes
                // and rather configure it from there
                mefContainer = mefConfiguration.CreateContainer();

                loadedPlugins = validAssemblies.SelectMany(a => a.GetTypes())
                    .Where(v => typeof(IModule).IsAssignableFrom(v))
                    .Where(t => t != typeof(IModule) && !t.IsAbstract)
                    .ToList();

                var vss = loadedPlugins.Select(type => new ModuleInfo(type)).ToList();

                moduleCatalog = new ModuleCatalog();
                vss.ForEach(c => moduleCatalog.AddModule(c));

                return moduleCatalog;
            }

            /// <summary>
            /// Returns types from a given <paramref name="unityContainer"/> that are implementing the <typeparamref name="T"/> and (optinally) that
            /// are decorated with an attribute of the <paramref name="attributeType"/>
            /// </summary>
            /// <typeparam name="T">The type of the base class/interface</typeparam>
            /// <param name="unityContainer">The container where the types are searched for</param>
            /// <param name="attributeType">The type of the attribute that should be present</param>
            /// <returns>The types satisfying all the previously mentioned conditions</returns>
            private IEnumerable<Type> FilterUnityExportedTypesByTypeAndAttribute<T>(IUnityContainer unityContainer, Type attributeType = null)
            {
                var tmp = unityContainer.Registrations.Select(x => x.RegisteredType).Where(x => typeof(T).IsAssignableFrom(x) && !x.IsAbstract);
                if (attributeType != null)
                {
                    tmp = tmp.Where(x => Attribute.IsDefined(x, attributeType));
                }
                return tmp;
            }

            /// <summary>
            /// Initializes the modules
            /// </summary>
            protected override void InitializeModules()
            {
                base.InitializeModules();

                UnityContainer = Container.Resolve<IUnityContainer>();

                //Get All instances from mefContainer
                //The reason why only mefContainer is considered here is because unityContainer should be used mainly for
                //factoryRegistrations and dependencies used by these factories are not registered yet
                //On the other hand, MEF is used for easy&fast way to export dependency-less plugins so it is safe to instantiate them at any moment
                var placementAlgorithms = mefContainer.GetExports<IPlacementAlgorithm>();
                var minimumBoundingBoxFinders = mefContainer.GetExports<IMinimumBoundingBoxFinder>();

                //Get All exported types (from both mefContainer & unityContainer)
                var placementAlgorithmTypes = placementAlgorithms.Select(x => x.GetType())
                    .Concat(FilterUnityExportedTypesByTypeAndAttribute<IPlacementAlgorithm>(UnityContainer));
                var minimumBoundingBoxFinderTypes = minimumBoundingBoxFinders.Select(x => x.GetType())
                    .Concat(FilterUnityExportedTypesByTypeAndAttribute<IMinimumBoundingBoxFinder>(UnityContainer));

                //[SelfContained] - again, no dependencies => should be exported using MEF
                var selfContainedPlacementAlgorithms = placementAlgorithms.Where(x => Attribute.IsDefined(x.GetType(), typeof(SelfContainedAttribute)));
                var selfContainedMinimumBoundingBoxFinders = minimumBoundingBoxFinders.Where(x => Attribute.IsDefined(x.GetType(), typeof(SelfContainedAttribute)));

                //[PartiallyContained] are registered with Unity because they have dependencies that are not known yet, therefore we only deal with types (not instances)
                var partiallyContainedPlacementAlgorithms = FilterUnityExportedTypesByTypeAndAttribute<IPlacementAlgorithm>(UnityContainer, typeof(PartiallyContainedAttribute));
                var partiallyContainedMinimumBoundingBoxFinders = FilterUnityExportedTypesByTypeAndAttribute<IMinimumBoundingBoxFinder>(UnityContainer, typeof(PartiallyContainedAttribute));

                var AllPlacementAlgorithmTypes = placementAlgorithmTypes.Concat(partiallyContainedPlacementAlgorithms).Distinct();
                var AllMinimumBoundingBoxFinderTypes = minimumBoundingBoxFinderTypes.Concat(partiallyContainedMinimumBoundingBoxFinders).Distinct();

                PlacementAlgorithmTypes = AllPlacementAlgorithmTypes
                    .Concat(selfContainedPlacementAlgorithms.Select(x => x.GetType()))
                    .Distinct();

                MinimumBoundingBoxFinderTypes = AllMinimumBoundingBoxFinderTypes
                    .Concat(selfContainedMinimumBoundingBoxFinders.Select(x => x.GetType()))
                    .Distinct();
            }

            public void Dispose()
            {
                mefContainer.Dispose();
            }

            /// <summary>
            /// Types of all the placement algorithm that were loaded
            /// </summary>
            public IEnumerable<Type> PlacementAlgorithmTypes { get; private set; }

            /// <summary>
            /// Types of all the minimum bounding box finders that were loaded
            /// </summary>
            public IEnumerable<Type> MinimumBoundingBoxFinderTypes { get; private set; }

            /// <summary>
            /// The unity container
            /// </summary>
            private IUnityContainer UnityContainer { get; set; }

            /// <summary>
            /// Catalog with modules
            /// </summary>
            private IModuleCatalog moduleCatalog;

            /// <summary>
            /// Types of all the loaded plugins
            /// </summary>
            private IEnumerable<Type> loadedPlugins;
            
            /// <summary>
            /// MEF container
            /// </summary>
            private CompositionHost mefContainer;
        }
    }
}
