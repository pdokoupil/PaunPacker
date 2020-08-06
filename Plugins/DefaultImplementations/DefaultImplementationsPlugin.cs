using System.Windows.Controls;
using PaunPacker.Core.ImageProcessing;
using PaunPacker.Core.ImageProcessing.ImageProcessors;
using PaunPacker.Core.Packing.MBBF;
using PaunPacker.Core.Packing.Placement;
using PaunPacker.Core.Packing.Placement.Guillotine;
using PaunPacker.Core.Packing.Placement.MaximalRectangles;
using PaunPacker.Core.Packing.Sorting;
using PaunPacker.GUI.WPF.Common;
using PaunPacker.GUI.WPF.Common.Attributes;
using PaunPacker.Plugins.ViewModels;
using PaunPacker.Plugins.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Unity;

namespace PaunPacker.Plugin
{
    /// <summary>
    /// Serves as a plugin entry point for a plugin that exports default implementations from PaunPacker
    /// </summary>
    [PluginMetadata(typeof(DefaultImplementationsPlugin), nameof(DefaultImplementationsPlugin),
        "The plugin with default implementations, provide implementations of several packing algorithms, " +
        "image sorters, processing tools, metadata writers. " +
        "It can be considered as basic toolset",
        "PaunPacker",
        "1.0.0.0",
        typeof(PowerOfTwoSizePacker),
        typeof(FixedSizePacker),
        typeof(UnknownSizePacker),
        typeof(ByHeightAndWidthImageSorter),
        typeof(ByHeightAndWidthImageSorterDesc),
        typeof(PreserveOrderImageSorter),
        typeof(BLAlgorithmPacker),
        typeof(Trimmer),
        typeof(CroppingTrimmer),
        typeof(PaddingAdder),
        typeof(HeuristicBackgroundRemover),
        typeof(Extruder))]
    [ExportedTypeMetadata(typeof(Extruder), nameof(Extruder), "Default implementation of extrude image processor", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(ColorTypeChanger), nameof(ColorTypeChanger), "Default implementation of ColorTypeChangeBase", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(HeuristicBackgroundRemover), nameof(HeuristicBackgroundRemover), "Implementation of BackgroundRemover that is using heuristic", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(Trimmer), nameof(Trimmer), "Default implementation of Trimmer", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(CroppingTrimmer), nameof(CroppingTrimmer), "Default implementation of Cropper", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(PaddingAdder), nameof(PaddingAdder), "Default implementation of PaddingAdder", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(FixedSizePacker), nameof(FixedSizePacker), "Packer that try to pack into containers of size WxH where W & H are both fixed and known in advance", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(PowerOfTwoSizePacker), nameof(PowerOfTwoSizePacker), "Packer that try to pack into containers of size WxH where W & H are both PoT", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(UnknownSizePacker), nameof(UnknownSizePacker), "Packer that try to pack into containers of size WxH where W & H are both unknown", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(BLAlgorithmPacker), nameof(BLAlgorithmPacker), "Implementation of Bottom-Left Packing Algorithm", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(MaximalRectanglesAlgorithm), nameof(MaximalRectanglesAlgorithm), "Implementation of Maximal Rectangles Packing Algorithm", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(GuillotineBestAreaFitAlgorithm), nameof(GuillotineBestAreaFitAlgorithm), "Best area fit implementation of Guillotine Packing Algorithm", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(MaxRectsFreeRectanglePostProcessor), nameof(MaxRectsFreeRectanglePostProcessor), "Implementation of free rectangle post-processor used in Maximal Rectangles algorithm", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(MaxRectsFreeRectangleSortedMerger), nameof(MaxRectsFreeRectangleSortedMerger), "Implementation of free rectangle merger used in Maximal Rectangles algorithm", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(MaxRectsFreeRectangleSplitter), nameof(MaxRectsFreeRectangleSplitter), "Implementation of free rectangle splitter used in Maximal Rectangles algorithm", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(ByHeightAndWidthImageSorter), nameof(ByHeightAndWidthImageSorter), "Image sorter that sorts images by Height and then Width (both in ascending order)", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(ByHeightAndWidthImageSorterDesc), nameof(ByHeightAndWidthImageSorterDesc), "Image sorter that sorts images by Height and then Width (both in descending order)", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(PreserveOrderImageSorter), nameof(PreserveOrderImageSorter), "Image sorter that actually does nothing - identity. Is usable in scenarios where you have to provide image sorter to some subroutine but you do not want the order to be manipulated", "PaunPacker", "1.0.0.0")]
    public class DefaultImplementationsPlugin : IModule
    {
        /// <summary>
        /// Performs initialization and exports the types
        /// </summary>
        /// <param name="containerProvider">Container provider given by the Prism</param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
            IoC = containerProvider.Resolve<IUnityContainer>();
            EventAggregator = containerProvider.Resolve<IEventAggregator>();

            //Export simple types that are defined in PaunPacker.Core
            IoC.RegisterType<HeuristicBackgroundRemover>();
            IoC.RegisterType<PowerOfTwoSizePacker>();
            IoC.RegisterType<UnknownSizePacker>();

            IoC.RegisterFactory<BLAlgorithmPacker>((unityContainer) =>
            {
                var sorter = unityContainer.Resolve<IImageSorter>();
                if (sorter == null)
                {
                    return new BLAlgorithmPacker();
                }
                return new BLAlgorithmPacker(sorter);
            });

            IoC.RegisterType<MaximalRectanglesAlgorithm>();
            IoC.RegisterType<GuillotineBestAreaFitAlgorithm>();
            IoC.RegisterType<MaxRectsFreeRectanglePostProcessor>();
            IoC.RegisterType<MaxRectsFreeRectangleSortedMerger>();
            IoC.RegisterType<MaxRectsFreeRectangleSplitter>();

            IoC.RegisterType<ByHeightAndWidthImageSorter>();
            IoC.RegisterType<ByHeightAndWidthImageSorterDesc>();
            IoC.RegisterType<PreserveOrderImageSorter>();

            //FixedSize packers are handled directly from the PaunPacker.GUI (their GUI is MainWindow)

            //Export types that are accompanied by the GUI
            //Extruder
            var extruderView = new ExtruderView()
            {
                DataContext = new ExtruderViewModel(IoC)
            };
            //ColorTypeChanger
            //Trimmer
            var trimmerView = new TrimmerView()
            {
                DataContext = new TrimmerViewModel(IoC)
            };
            //CroppingTrimmer
            var croppingTrimmerView = new CroppingTrimmerView()
            {
                DataContext = new CroppingTrimmerViewModel(IoC)
            };
            //PaddingAdder
            var paddingAdderView = new PaddingAdderView()
            {
                DataContext = new PaddingAdderViewModel(IoC)
            };

            //Register the views
            IoC.RegisterInstance<UserControl>(PluginViewWiring.GetViewName(typeof(Extruder)), extruderView);
            IoC.RegisterInstance<UserControl>(PluginViewWiring.GetViewName(typeof(Trimmer)), trimmerView);
            IoC.RegisterInstance<UserControl>(PluginViewWiring.GetViewName(typeof(CroppingTrimmer)), croppingTrimmerView);
            IoC.RegisterInstance<UserControl>(PluginViewWiring.GetViewName(typeof(PaddingAdder)), paddingAdderView);
        }

        /// <summary>
        /// Implementation of the Prism's Register types
        /// </summary>
        /// <param name="containerRegistry">The container registry</param>
        /// <remarks>Does not do anything</remarks>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }

        /// <summary>
        /// The IoC Container used for exports and imports
        /// </summary>
        public IUnityContainer IoC
        {
            get; private set;
        }

        /// <summary>
        /// The event aggregator used for communication with host application
        /// </summary>
        public IEventAggregator EventAggregator
        {
            get; private set;
        }
    }
}
