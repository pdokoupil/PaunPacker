using System.ComponentModel;
using Unity;

namespace PaunPacker.Plugins.ViewModels
{
    /// <summary>
    /// View model wired to the <see cref="GeneticMinimumBoundingBoxFinderPlugin"/>'s view 
    /// </summary>
    class GeneticMinimumBoundingBoxFinderViewModel// : ViewModelBase
    {
        /// <summary> 
        /// Constructs the view model and registers The <see cref="GeneticMinimumBoundingBoxFinder"/> into the <paramref name="ioc"/>
        /// </summary>
        /// <param name="ioc">The unity container</param>
        public GeneticMinimumBoundingBoxFinderViewModel(IUnityContainer ioc)
        {
                
            IoC = ioc;

            //create it with initial parameters
            maxIterations = 123;
            populationSize = 456;

            
            IoC.RegisterFactory<GeneticMinimumBoundingBoxFinder>((_) =>
            {
                return new GeneticMinimumBoundingBoxFinder(MaxIterations, PopulationSize);
            });
        }

        /// <summary>
        /// The population bound to the text box field within the GUI
        /// </summary>
        public int PopulationSize
        {
            get
            {
                return populationSize;
                //return ExportedMinimumBoundingBoxFinder.PopulationSize;
            }
            set
            {
                if (value <= 2)
                {
                    populationSize = 2;
                }
                else
                {
                    populationSize = value;
                }
                //ExportedMinimumBoundingBoxFinder.PopulationSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PopulationSize)));
            }
        }

        /// <summary>
        /// The population bound to the text box field within the GUI
        /// </summary>
        public int MaxIterations
        {
            get
            {
                return maxIterations;
                //return ExportedMinimumBoundingBoxFinder.MaxIterations;
            }
            set
            {
                maxIterations = value;
                //ExportedMinimumBoundingBoxFinder.MaxIterations = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxIterations)));
            }
        }

        /// <summary>
        /// The Unity container that was obtained (initially) from the PaunPacker.GUI
        /// </summary>
        public IUnityContainer IoC
        {
            get; private set;
        }

        /// <summary>
        /// The PropertyChangedEvent used to notify the view about data changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <see cref="PopulationSize"/>
        private int populationSize;

        /// <see cref="MaxIterations"/>
        private int maxIterations;
    }
}

