using System;
using System.Collections.Generic;
using System.Linq;
using PaunPacker.Core.Packing;
using PaunPacker.Core.Packing.MBBF;
using PaunPacker.Core.Packing.Placement;
using PaunPacker.Core.Packing.Sorting;
using PaunPacker.Core.Packing.Utils;
using PaunPacker.Core.Types;
using PaunPacker.GUI.WPF.Common.Attributes;

namespace PaunPacker.Plugins
{
    //Algorithm based on On genetic algorithms for the packing of polygons Stefan Jakobs, from now on, denoted as [1]   (href: http://www.cs.nott.ac.uk/~pszgxk/aim/2009/reading/ga_representation_2.pdf)
    //also some parts (trim loss..) are taken from Hybrid genetic algorithm and simulated annealing for two-dimensionalnon-guillotine rectangular packing problems (href: https://www.inf.utfsm.cl/~mcriff/SP/articulo3.pdf)
    //Some things are modified: fitness function only based on area, first Individual from population (Height then Width)             
    //In the folowing class, I sometimes do not stick with convetions used in the rest of the solution, but rather I incline to use Genetic algorithms terminology

    //Some of the notes: One of the possible changes would be to replace all the methods (looks more like procedural interface) with some generic and more general interface usable with Genetic algorithms
    //i.e. to provide classes/interfaces like: GeneticOperator, MutationOperator, CrossOverOperator, ... etc.. and add methods to all of those + add methods to Individual class
    //However, as I try to simmulate the possibility to just provide custom "black-box" Packer to whole application and I will not (or better to say it is not the main objective of this work) write
    //another genetic algorithms, I decided to left it as it is and do not pollute this implementation with (rather unnecessary) generalizations.

    // TODO: clean up code, try to improve performance and somehow try to (? maybe) encapsulate PackingPattern & Individual class, also decide whether it is good idea to return new Individual from all the operators
    //or if it would be better to modify given individual

    /// <summary>
    /// Packing algorithm based on genetic algorithm
    /// See <a href="http://www.cs.nott.ac.uk/~pszgxk/aim/2009/reading/ga_representation_2.pdf">On genetic algorithms for the packing of polygons</a> for some of the implementation (algorithm) details.
    /// </summary>
    [ExportedTypeMetadata(typeof(GeneticMinimumBoundingBoxFinder), nameof(GeneticMinimumBoundingBoxFinder), "Implementation of genetic packing algorithm", "PaunPacker", "1.0.0.0")]
    [PartiallyContained]
    public class GeneticMinimumBoundingBoxFinder : IMinimumBoundingBoxFinder
    {
        /// <summary>
        /// Encoding of the Pattern. The Packing Pattern is encoded as an permutation of the input images + width of the packing and + information about rotation of the images
        /// </summary>
        private class PackingPattern
        {
            public PackingPattern(int [] permutation, int packingWidth)
            {
                this.permutation = permutation ?? throw new ArgumentNullException($"{nameof(permutation)} is invalid !");
                this.packingWidth = packingWidth;
                this.isRotated = new bool[permutation.Length];
            }
            public int[] permutation; //we have images, and image at index 'i' in IEnumerable<PPRect> images will go to position permutation[i].
            public int packingWidth;
            public bool[] isRotated; //each image can be either no rotated or rotated by 90 degrees (does not matter if clockwise / counterclockwise as the effect to the packing would be the same)
        }

        /// <summary>
        /// Class representing an individual (in Genetic algorithms terminology)
        /// </summary>
        private class Individual : ICloneable
        {
            /// <summary>
            /// The pattern corresponding to the individual
            /// </summary>
            public PackingPattern pattern;

            /// <summary>
            /// The fitness value of the individual
            /// </summary>
            public double fitness;

            /// <summary>
            /// Creates a deep clone of the Individual
            /// </summary>
            /// <returns></returns>
            public Individual Clone()
            {
                Individual res = new Individual
                {
                    fitness = fitness,
                    pattern = new PackingPattern(new int[pattern.permutation.Length], pattern.packingWidth)
                };
                Array.Copy(pattern.permutation, res.pattern.permutation, pattern.permutation.Length);
                Array.Copy(pattern.isRotated, res.pattern.isRotated, pattern.isRotated.Length);
                return res;
            }

            object ICloneable.Clone()
            {
                return Clone();
            }
        }

        /// <summary>
        /// Constructs a new <see cref="GeneticMinimumBoundingBoxFinder"/> with a given number of maximum iterations and population size
        /// </summary>
        /// <param name="maxIterations">Number of iteratios, has to be at least 0</param>
        /// <param name="populationSize">Population size has to be at least 2</param>
        /// <exception cref="ArgumentOutOfRangeException">Is thrown when <paramref name="maxIterations"/> is less than 0 or <paramref name="populationSize"/> is less than two</exception>
        public GeneticMinimumBoundingBoxFinder(int maxIterations = DefaultMaximumOfIterations, int populationSize = DefaultPopulationSize)
        {
            if (maxIterations < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(maxIterations)} has to be at least one");
            }

            if (populationSize < 2)
            {
                throw new ArgumentOutOfRangeException($"{nameof(maxIterations)} has to be at least two");
            }

            MaxIterations = maxIterations;
            PopulationSize = populationSize;
            this.randomGenerator = new Random();
        }

        /// <summary>
        /// Decodes the PackingPattern i.e. constructs the resulting Packing.
        /// </summary>
        /// <param name="packing">The PackingPattern to be decoded</param>
        /// <param name="rects">Corresponding rects</param>
        /// <returns>Resulting Packing</returns>
        private static PackingResult Decode(ref PackingPattern packing, IEnumerable<PPRect> rects)
        {
            BLAlgorithmPacker blAlgorithm = new BLAlgorithmPacker(new PreserveOrderImageSorter());
            var rectsPermuted = Permute(packing.permutation, rects);
            return blAlgorithm.PlaceRects(packing.packingWidth, Int32.MaxValue, rectsPermuted);
        }
        
        /// <summary>
        /// Applies given permutation to the given <paramref name="rects"/>
        /// </summary>
        /// <param name="permutation">Permutation that determines the order of rects</param>
        /// <param name="rects">rects to be permuted</param>
        /// <returns>Permutes <paramref name="rects"/> with respect to <paramref name="permutation"/></returns>
        private static IEnumerable<PPRect> Permute(int[] permutation, IEnumerable<PPRect> rects)
        {
            return rects.Select((image, index) => new { image, index }).OrderBy(x => permutation[x.index]).Select(x => x.image);
        }

        /// <summary>
        /// Performs rotation of the given <paramref name="rects"/>, based on the given array <paramref name="isRotated"/>
        /// </summary>
        /// <param name="isRotated">Element at i-th index represents whether i-th image is rotated (by 90 degrees)</param>
        /// <param name="rects">Corresponding rects</param>
        /// <returns>Rotated <paramref name="rects"/></returns>
        private IEnumerable<PPRect> Rotate(bool[] isRotated, IEnumerable<PPRect> rects)
        {
            return rects.Select((image, index) =>
            {
                if (isRotated[index])
                {
                    return image.RotateClockWiseBy90Degrees();
                }
                return image;
            });
        }

        //Would make more sense to pass GeneticDataStructure instead, be we would not like to spend time of another decode call ..
        private static double FitnessFunction(PackingResult result)
        {
            return PackingAreaFitnessFunction(result);
        }

        /// <summary>
        /// Calculates the packing area fitness function
        /// </summary>
        /// <param name="result">The <see cref="PackingResult"/> for which the packing area fitness will be calculated</param>
        /// <returns>The packing area fitness function</returns>
        private static double PackingAreaFitnessFunction(PackingResult result)
        {
            if (result == null)
            {
                return 1.0 / double.MaxValue;
            }
            return 1.0 / (result.Width * result.Height);
        }

        //Another fitness function ...
        /// <summary>
        /// Calculates the packing trim loss fitness function
        /// </summary>
        /// <param name="result">The <see cref="PackingResult"/> for which the trim loss fitness will be calculated</param>
        /// <returns>The packing trim loss fitness function</returns>
        private static double PackingTrimLossFitnessFunction(PackingResult result)
        {
            return 1.0 / (Epsilon + TrimLossValue(result));
        }

        /// <summary>
        /// Calculates a trim loss value for a given <paramref name="result"/>
        /// </summary>
        /// <param name="result">The <see cref="PackingResult"/> for which the trimm loss will be calculated</param>
        /// <returns>The trim loss value</returns>
        private static double TrimLossValue(PackingResult result)
        {
            if (result == null)
                return double.MaxValue;
            double areaOfWholePacking = result.Width * result.Height;
            double sumOfAreasOfSubRects = result.Rects.Select(x => x.Width * x.Height).Sum();
            return (areaOfWholePacking - sumOfAreasOfSubRects) / areaOfWholePacking;
        }

        /// <summary>
        /// Checks whether a value <paramref name="valueToTest"/> lies in the interval [<paramref name="intervalStart"/>, <paramref name="intervalEnd"/>)
        /// </summary>
        /// <param name="valueToTest">The value to be tested</param>
        /// <param name="intervalStart">Inclusive start of the interval</param>
        /// <param name="intervalEnd">Exclusive end of the interval</param>
        /// <returns>True if it lies in the interval, false otherwise</returns>
        private static bool LiesInLeftInclusiveInterval(double valueToTest, double intervalStart, double intervalEnd)
        {
            return (valueToTest >= intervalStart && valueToTest < intervalEnd);
        }

        /// <summary>
        /// Selects individual from population with respect to some probability
        /// </summary>
        /// <param name="population">Population to select from</param>
        /// <returns>Selected individual</returns>
        private int SelectIndividual(Individual [] population)
        {
            //Proportional selection
            (double Start, double End)[] intervals = new(double Start, double End)[population.Length];
            double[] probabilities = new double[population.Length];

            double sumOfFitnessValues = population.Select(x => x.fitness).Sum(); // maybe better to pass it to this function as parameter (faster..)

            //Fill in probabilities
            for (int i = 0; i < population.Length; i++)
            {
                probabilities[i] = population[i].fitness / sumOfFitnessValues;
            }

            //We are about to select 2 random individuals with probability pi, pj
            double probabilityForIndividual = randomGenerator.NextDouble();
            
            int indexOfIndividual = -1;
            
            intervals[0].Start = 0; //reduce code duplication
            intervals[0].End = probabilities[0];
            if (LiesInLeftInclusiveInterval(probabilityForIndividual, intervals[0].Start, intervals[0].End))
            {
                indexOfIndividual = 0;
            }

            for (int i = 1; i < population.Length; i++)
            {
                intervals[i].Start = intervals[i - 1].End;
                intervals[i].End = intervals[i - 1].End + probabilities[i];
                if (LiesInLeftInclusiveInterval(probabilityForIndividual, intervals[i].Start, intervals[i].End))
                {
                    indexOfIndividual = i;
                }
            }

            return indexOfIndividual;
        }

        /// <summary>
        /// Performs crossover of two individuals
        /// </summary>
        /// <param name="first">First individual</param>
        /// <param name="second">Second individual</param>
        /// <returns>New individual that is "children" of <paramref name="first"/> and <paramref name="second"/></returns>
        private Individual Crossover(Individual first, Individual second)
        {
            Individual newIndividual = new Individual();

            //We randomly choose startIndex and copy amountOfCopied elements from first permutation
            //to the new permutation (start at index startIndex, i.e. range [startIndex, startIndex + amountOfCopied)
            //the rest is taken from second permutation
            int permutationLength = first.pattern.permutation.Length;
            int startIndex = randomGenerator.Next(0, permutationLength);
            int amountOfCopied = randomGenerator.Next(1, permutationLength - startIndex + 1);
            Individual randomIndividual = randomGenerator.Next(0, 2) == 0 ? first : second; //randomly select one of the two given individuals
            newIndividual.pattern = new PackingPattern(new int[permutationLength],randomIndividual.pattern.packingWidth);

            bool[] indexAlreadyUsed = new bool[permutationLength];
            for (int i = 0; i < amountOfCopied; i++)
            {
                newIndividual.pattern.permutation[i] = first.pattern.permutation[startIndex + i];
                indexAlreadyUsed[first.pattern.permutation[startIndex + i]] = true;
            }


            int j = 0;
            for (int i = amountOfCopied; i < permutationLength; i++)
            {
                while (indexAlreadyUsed[second.pattern.permutation[j]])
                {
                    j++;
                }
                newIndividual.pattern.permutation[i] = second.pattern.permutation[j];
                indexAlreadyUsed[second.pattern.permutation[j]] = true;
            }

            return newIndividual;
        }

        /// <summary>
        /// Applies rotate-mutation operator to the given individual
        /// </summary>
        /// <param name="individual">Individual to be mutated</param>
        /// <returns>Mutated <paramref name="individual"/></returns>
        private Individual Mutate(Individual individual)
        {
            Individual mutatedIndividual = individual.Clone();
            const double rotationProbability = 0.25; //make as member?
            int permutationLength = individual.pattern.permutation.Length;
            for (int i = 0; i < permutationLength; i++)
            {
                double probability = randomGenerator.NextDouble();
                if (probability < rotationProbability)
                {
                    mutatedIndividual.pattern.isRotated[i] = true;
                }
            }
            return mutatedIndividual;
        }

        /// <summary>
        /// Applies general mutation operator to the given individual
        /// </summary>
        /// <param name="individual">Individual to be mutated</param>
        /// <returns>Mutated <paramref name="individual"/></returns>
        private Individual MutateNormal(Individual individual)
        {
            //there are many ways to implement mutation
            return RandomlyInvertPartOfPermutation(individual);
        }

        /// <summary>
        /// Mutates Individual by inverting some random, continuous part of its permutation
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        private Individual RandomlyInvertPartOfPermutation(Individual individual)
        {
            Individual mutatedIndividual = individual.Clone();
            int permutationLength = individual.pattern.permutation.Length;
            int blockStartIndex = randomGenerator.Next(0, permutationLength);
            int blockEndIndex = randomGenerator.Next(blockStartIndex, permutationLength);
            double mutationProbabilityTreshold = 0.25;
            double mutationProbability = randomGenerator.NextDouble();

            //No mutation
            if (mutationProbability >= mutationProbabilityTreshold)
            {
                return mutatedIndividual;
            }

            //Perform the mutation - reversing permutation ... (part of it)
            for (int i = blockStartIndex; i <= blockEndIndex; i++)
            {
                int indexDiff = i - blockStartIndex;
                mutatedIndividual.pattern.permutation[i] = individual.pattern.permutation[blockEndIndex - indexDiff];
            }

            return mutatedIndividual;
        }

        /// <summary>
        /// Calculates (fill's in individual's fitness attribute) fitness value for a given individual and corresponding rects.
        /// Fitness function is calculated with respect to the <paramref name="rects"/> and fixed width of the packing
        /// </summary>
        /// <param name="individual">Given individual</param>
        /// <param name="rects">Rects with respect which we will calculate fitness value</param>
        /// <returns>Individual with filled fitness attribute</returns>
        private static Individual EvaluateIndividualKnownWidth(Individual individual, IEnumerable<PPRect> rects)
        {
            Individual evaluatedIndividual = individual.Clone();
            PackingResult result = Decode(ref evaluatedIndividual.pattern, rects);
            evaluatedIndividual.fitness = FitnessFunction(result);
            return evaluatedIndividual;
        }

        /// <summary>
        /// Calculates (fill's in individual's fitness attribute) fitness value for a given individual and corresponding rects.
        /// Fitness function is calculated with respect to the <paramref name="rects"/> and uknown width of the packing
        /// </summary>
        /// <param name="individual">Given individual</param>
        /// <param name="rects">Rects with respect which we will calculate fitness value</param>
        /// <returns>Individual with filled fitness attribute</returns>
        private static Individual EvaluateIndividual(Individual individual, IEnumerable<PPRect> rects)
        {
            if (individual.pattern.packingWidth == -1) //not evaluated yet, initial population
            {
                (int BestWidth, double BestFitness, int [] BestPermutation) = (0, 0.0, null);

                foreach (int widthToTry in SingleDimensionEnumerator.GetWidth(rects))
                {
                    individual.pattern.packingWidth = widthToTry;
                    individual = EvaluateIndividualKnownWidth(individual, rects);
                    if (individual.fitness > BestFitness)
                    {
                        BestFitness = individual.fitness;
                        BestWidth = widthToTry;
                        BestPermutation = individual.pattern.permutation;
                    }
                }

                individual.fitness = BestFitness;
                individual.pattern.packingWidth = BestWidth;
                individual.pattern.permutation = BestPermutation;
                return individual;
            }
            else
            {
                return EvaluateIndividualKnownWidth(individual, rects);
            }
        }

        private int [] GetSortByWidthDescPermutation(IEnumerable<PPRect> rects, int numOfRects)
        {
            var sortByWidthDesc = rects.Select((image, oldIndex) => new { Image = image, OldIndex = oldIndex })
                .OrderByDescending(x => x.Image.Width)
                .Select((x, newIndex) => new { x.OldIndex, NewIndex = newIndex });
            int[] sortByWidthDescPermutation = new int[numOfRects];

            foreach (var item in sortByWidthDesc)
            {
                sortByWidthDescPermutation[item.OldIndex] = item.NewIndex;
            }

            return sortByWidthDescPermutation;
        }

        private static int [] GetSortByHeightDescThenWidthDescPermutation(IEnumerable<PPRect> rects, int numOfRects)
        {
            //The ByHeightAndWidthImageSorterDesc cannot be reused here, as I am working with IEnumerable<(PPRect, int)> instead of IEnumerable<PPRect>
            var sortByHeightDescThenWidthDesc = rects.Select((image, oldIndex) => new { image.Height, image.Width, OldIndex = oldIndex })
                .OrderByDescending(x => x.Height)
                .ThenByDescending(x => x.Width)
                .Select((x, newIndex) => new { x.Height, x.Width, x.OldIndex, NewIndex = newIndex });
            int[] sortByHeightDescThenWidthDescPermutation = new int[numOfRects];
            
            foreach (var item in sortByHeightDescThenWidthDesc)
            {
                sortByHeightDescThenWidthDescPermutation[item.OldIndex] = item.NewIndex;
            }

            return sortByHeightDescThenWidthDescPermutation;
        }

        /// <summary>
        /// Calculates the initial population
        /// </summary>
        /// <param name="images">Images to be packed</param>
        /// <param name="numOfImages">Number of images (to prevent IEnumerable's Count() recalculation)</param>
        /// <returns>The initial population</returns>
        private Individual [] GetInitialPopulation(IEnumerable<PPRect> images, int numOfImages)
        {
            Individual[] initialPopulation = new Individual[PopulationSize];

            int[] randomPermutation = null;

            //In the paper: On genetic algorithms for the packing of polygons, the first individual of population is sorted by Width desc
            //initialPopulation[0] = new Individual() { pattern = new PackingPattern(getSortByWidthDescPermutation(images), -1), fitness = 0.0 };
            //However, I decided to sort by Height then by Width (both desc), as proposed in another papers (usually on the BL algorithms)
            //study the impacts ... ?
            initialPopulation[0] = new Individual() { pattern = new PackingPattern(GetSortByHeightDescThenWidthDescPermutation(images, numOfImages), -1), fitness = 0.0 };
            for (int i = 1; i < PopulationSize; i++)
            {
                randomPermutation = Enumerable.Range(0, numOfImages).OrderBy(x => Guid.NewGuid()).ToArray();
                initialPopulation[i] = new Individual() { pattern = new PackingPattern(randomPermutation, -1), fitness = 0.0 };
            }

            double sumOfFitnessValues = 0.0;

            //We have initialized all the permutations, now let's calculate fitness values
            for (int i = 0; i < PopulationSize; i++)
            {
                initialPopulation[i] = EvaluateIndividual(initialPopulation[i], images);
                sumOfFitnessValues += initialPopulation[i].fitness;
            }

            return initialPopulation;
        }

        /// <summary>
        /// Returns the index of the worst (measured by fitness value) individual from a given <paramref name="population"/>
        /// </summary>
        /// <param name="population">The population to choose from</param>
        /// <returns>The index of the worst individual in <paramref name="population"/></returns>
        private int GetIndexOfWostIndividual(Individual [] population)
        {
            int indexOfWorstIndividual = -1;
            double worstFitness = double.MaxValue;
            for (int i = 0; i < population.Length; i++)
            {
                if (population[i].fitness < worstFitness)
                {
                    indexOfWorstIndividual = i;
                    worstFitness = population[i].fitness;
                }
            }
            return indexOfWorstIndividual;
        }

        /// <summary>
        /// Returns the index of the best (measured by fitness value) individual from a given <paramref name="population"/>
        /// </summary>
        /// <param name="population">The population to choose from</param>
        /// <returns>The index of the best individual in <paramref name="population"/></returns>
        private int GetIndexOfBestIndividual(Individual [] population)
        {
            int indexOfBestIndividual = -1;
            double bestFitness = 0;
            for (int i = 0; i < population.Length; i++)
            {
                if (population[i].fitness > bestFitness)
                {
                    indexOfBestIndividual = i;
                    bestFitness = population[i].fitness;
                }
            }
            return indexOfBestIndividual;
        }

        /// <inheritdoc />
        public PackingResult FindMinimumBoundingBox(IEnumerable<PPRect> images, System.Threading.CancellationToken cancellationToken = default)
        {
            Progress = 0; //if executed multiple times
            int numOfImages = images.Count();

            if (numOfImages == 0)
                return new PackingResult(0, 0, Enumerable.Empty<PPRect>());

            Individual[] population = GetInitialPopulation(images, numOfImages); //or deep copy?
            int iteration = 0;
            while (iteration < MaxIterations)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    int indexOfBest = GetIndexOfBestIndividual(population);
                    return Decode(ref population[indexOfBest].pattern, images);
                    //cancellationToken.ThrowIfCancellationRequested();
                }

                int indexOfFirstIndividual = SelectIndividual(population);
                int indexOfSecondIndividual = SelectIndividual(population);
                while (indexOfFirstIndividual == indexOfSecondIndividual)
                    indexOfSecondIndividual = SelectIndividual(population);

                Individual newIndividual = Crossover(population[indexOfFirstIndividual], population[indexOfSecondIndividual]);

                newIndividual = MutateNormal(newIndividual);
                newIndividual = Mutate(newIndividual);

                newIndividual = EvaluateIndividual(newIndividual, images);
                int indexOfWorstIndividual = GetIndexOfWostIndividual(population);
                population[indexOfWorstIndividual] = newIndividual;

                iteration++;
                Progress = (int)((double)iteration / (double)MaxIterations * 100.0);
                ProgressChange?.Invoke(this, Progress);
            }

            //return BEST Individual
            int indexOfBestIndividual = GetIndexOfBestIndividual(population);
            return Decode(ref population[indexOfBestIndividual].pattern, images);
        }

        private readonly Random randomGenerator;

        //private readonly int populationSize;
        //private readonly int maxIterations;

        /// <summary>
        /// The size of the population
        /// </summary>
        public int PopulationSize { get; set; }

        /// <summary>
        /// The number of iterations
        /// </summary>
        public int MaxIterations { get; set; }

        private const double Epsilon = 2.2204e-16;
        private const int DefaultPopulationSize = 20;
        private const int DefaultMaximumOfIterations = 100;

        /// <inheritdoc />
        public int Progress { get; private set; }

        /// <inheritdoc />
        public bool ReportsProgress => true;

        /// <inheritdoc />
        public event Action<object, int> ProgressChange;
    }
}
