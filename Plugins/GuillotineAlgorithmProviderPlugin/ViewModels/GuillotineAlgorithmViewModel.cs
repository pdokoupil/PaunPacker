using System;
using System.Collections.Generic;
using System.Linq;
using PaunPacker.Core.Packing.Placement.Guillotine;
using PaunPacker.Core.Packing.Placement.MaximalRectangles;
using Unity;

namespace PaunPacker.Plugins.ViewModels
{
    /// <summary>
    /// View model wired to the <see cref="GuillotineAlgorithmProviderPlugin"/>'s view 
    /// </summary>
    class GuillotineAlgorithmViewModel
    {
        /// <summary>
        /// Constructs the view model of guillotine algorithm
        /// </summary>
        /// <param name="freeRectangleExtractorTypes">The types of loaded free rectangle extractors</param>
        /// <param name="freeRectangleSplitterTypes">The types of loaded free rectangle splitters</param>
        /// <param name="freeRectangleMergerTypes">The types of loaded free rectangle mergers</param>
        /// <param name="rectOrientationSelectorTypes">The types of loaded rectangle orientation selectors</param>
        /// <param name="postProcessorTypes">The types of loaded free rectangle post processors</param>
        /// <param name="unityContainer">The unity container</param>
        public GuillotineAlgorithmViewModel(IEnumerable<Type> freeRectangleExtractorTypes,
            IEnumerable<Type> freeRectangleSplitterTypes,
            IEnumerable<Type> freeRectangleMergerTypes,
            IEnumerable<Type> rectOrientationSelectorTypes,
            IEnumerable<Type> postProcessorTypes,
            IUnityContainer unityContainer)
        {
            FreeRectangleExtractorVMs = freeRectangleExtractorTypes.Select(x => new FreeRectangleExtractorViewModel(x));
            FreeRectangleSplitterVMs = freeRectangleSplitterTypes.Select(x => new FreeRectangleSplitterViewModel(x));
            FreeRectangleMergerVMs = freeRectangleMergerTypes.Select(x => new FreeRectangleMergerViewModel(x));
            RectangleOrientationSelectorVMs = rectOrientationSelectorTypes.Select(x => new RectangleOrientationSelectorViewModel(x));
            FreeRectanglePostProcessorVMs = postProcessorTypes.Select(x => new FreeRectanglePostProcessorViewModel(x));

            unityContainer.RegisterFactory<IFreeRectangleExtractor>((_) =>
            {
                try
                {
                    var x = (IFreeRectangleExtractor)unityContainer.Resolve(SelectedFreeRectangleExtractorVM.FreeRectangleExtractorType);
                    x ??= new BestAreaFitFreeRectangleExtractor();
                    return x;
                }
                catch (ResolutionFailedException)
                {
                    //Return default implementation
                    return new BestAreaFitFreeRectangleExtractor();
                }
            });

            unityContainer.RegisterFactory<IFreeRectangleMerger>((_) =>
            {
                try
                {
                    var x = (IFreeRectangleMerger)unityContainer.Resolve(SelectedFreeRectangleMergerVM.FreeRectangleMergerType);
                    x ??= new GuillotineFreeRectangleSortedMerger();
                    return x;
                }
                catch (ResolutionFailedException)
                {
                    //Return default implementation
                    return new GuillotineFreeRectangleSortedMerger();
                }
            });

            unityContainer.RegisterFactory<IFreeRectangleSplitter>((_) =>
            {
                try
                {
                    var x = (IFreeRectangleSplitter)unityContainer.Resolve(SelectedFreeRectangleSplitterVM.FreeRectangleSplitterType);
                    x ??= new LongerAxisGuillotineFreeRectangleSplitter();
                    return x;
                }
                catch (ResolutionFailedException)
                {
                    //Return default implementation
                    return new LongerAxisGuillotineFreeRectangleSplitter();
                }
            });

            unityContainer.RegisterFactory<IFreeRectanglePostProcessor>((_) =>
            {
                try
                {
                    var x = (IFreeRectanglePostProcessor)unityContainer.Resolve(SelectedFreeRectanglePostProcessorVM.FreeRectanglePostProcessorType);
                    x ??= new MaxRectsFreeRectanglePostProcessor();
                    return x;
                }
                catch (ResolutionFailedException)
                {
                    //Return default implementation
                    return new MaxRectsFreeRectanglePostProcessor();
                }
            });

            unityContainer.RegisterFactory<IRectOrientationSelector>((_) =>
            {
                try
                {
                    var x = (IRectOrientationSelector)unityContainer.Resolve(SelectedRectangleOrientationSelectorVM.RectangleOrientationSelectorType);
                    x ??= new DummyRectOrientationSelector();
                    return x;
                }
                catch (ResolutionFailedException)
                {
                    //Return default implementation
                    return new DummyRectOrientationSelector();
                }
            });
        }


        /// <summary>
        /// The types implementing <see cref="IFreeRectangleExtractor"/> interface that were loaded by the PaunPacker.GUI
        /// </summary>
        public IEnumerable<FreeRectangleExtractorViewModel> FreeRectangleExtractorVMs { get; private set; }

        /// <summary>
        /// The types implementing <see cref="IFreeRectangleMerger"/> interface that were loaded by the PaunPacker.GUI
        /// </summary>
        public IEnumerable<FreeRectangleMergerViewModel> FreeRectangleMergerVMs { get; private set; }

        /// <summary>
        /// The types implementing <see cref="IFreeRectanglePostProcessor"/> interface that were loaded by the PaunPacker.GUI
        /// </summary>
        public IEnumerable<FreeRectanglePostProcessorViewModel> FreeRectanglePostProcessorVMs { get; private set; }

        /// <summary>
        /// The types implementing <see cref="IFreeRectangleSplitter"/> interface that were loaded by the PaunPacker.GUI
        /// </summary>
        public IEnumerable<FreeRectangleSplitterViewModel> FreeRectangleSplitterVMs { get; private set; }

        /// <summary>
        /// The types implementing <see cref="IRectOrientationSelector"/> interface that were loaded by the PaunPacker.GUI
        /// </summary>
        public IEnumerable<RectangleOrientationSelectorViewModel> RectangleOrientationSelectorVMs { get; private set; }

        /// <summary>
        /// The currently selected FreeRectangleExtractor
        /// </summary>
        public FreeRectangleExtractorViewModel SelectedFreeRectangleExtractorVM { get; set; }

        /// <summary>
        /// The currently selected FreeRectangleMerger
        /// </summary>
        public FreeRectangleMergerViewModel SelectedFreeRectangleMergerVM { get; set; }

        /// <summary>
        /// The currently selected FreeRectanglePostProcessor
        /// </summary>
        public FreeRectanglePostProcessorViewModel SelectedFreeRectanglePostProcessorVM { get; set; }

        /// <summary>
        /// The currently selected FreeRectangleSplitter
        /// </summary>
        public FreeRectangleSplitterViewModel SelectedFreeRectangleSplitterVM { get; set; }

        /// <summary>
        /// The currently selected RectangleOrientationSelector
        /// </summary>
        public RectangleOrientationSelectorViewModel SelectedRectangleOrientationSelectorVM { get; set; }
    }
}
