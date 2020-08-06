using System.Linq;
using System.Windows.Controls;
using PaunPacker.Core.Packing.Placement.Guillotine;
using PaunPacker.GUI.WPF.Common;
using PaunPacker.GUI.WPF.Common.Attributes;
using PaunPacker.GUI.WPF.Common.Events;
using PaunPacker.Plugins.Views;
using PaunPacker.Plugins.ViewModels;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Unity;

namespace PaunPacker.Plugins
{
    /// <summary>
    /// Main entry point of the plugin.
    /// This plugin exports the <see cref="GuillotineAlgorithmProviderPlugin"/> and provides a GUI for it
    /// This GUI was seperated from PaunPacker.Core and PaunPacker.GUI to separate Plugin
    /// </summary>
    [PluginMetadata(typeof(GuillotineAlgorithmProviderPlugin), nameof(GuillotineAlgorithmProviderPlugin), "Plugin exporting GuillotineAlgorithm and several dependencies for the GuillotineAlgorithm. It also exports a view that allows the user to select the dependencies to be used.",
        "PaunPacker",
        "1.0.0.0",
        typeof(GuillotinePlacementAlgorithm), typeof(BestAreaFitFreeRectangleExtractor), typeof(DummyRectOrientationSelector),
        typeof(GuillotineFreeRectangleSortedMerger), typeof(LongerAxisGuillotineFreeRectangleSplitter))]
    [ExportedTypeMetadata(typeof(GuillotinePlacementAlgorithm), nameof(GuillotinePlacementAlgorithm), "Fully parameterizable implementation of Guillotine algorithm", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(BestAreaFitFreeRectangleExtractor), nameof(BestAreaFitFreeRectangleExtractor), "Implementation of free rectangle extractor that selects according to best fit rule", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(DummyRectOrientationSelector), nameof(DummyRectOrientationSelector), "Dummy implementation of rectangle orientation selector that simply does not change the orientation at all", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(GuillotineFreeRectangleSortedMerger), nameof(GuillotineFreeRectangleSortedMerger), "Implementation of merger that merges rectangles into a sorted array (based by area)", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(LongerAxisGuillotineFreeRectangleSplitter), nameof(LongerAxisGuillotineFreeRectangleSplitter), "Implementation of free rectangle splitter that splits by the longer axis of the placed rectangle", "PaunPacker", "1.0.0.0")]

    public class GuillotineAlgorithmProviderPlugin : IModule
    {
        /// <summary>
        /// Creates the view and the view model
        /// The view is then registered so that the PaunPacker.GUI could load it later
        /// </summary>
        /// <remarks>Implementation of the Prism's <see cref="IModule"/> interface</remarks>
        /// <param name="containerProvider">Container provider given by the Prism</param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
            unityContainer = containerProvider.Resolve<IUnityContainer>();

            //Register known IFreeRectangleExtractors, IFreeRectangleSplitters, IFreeRectangleMergers, IRectOrientationSelectors
            //PostProcessors are not only related to guillotine so it is exported from DefaultImplementations ...

            unityContainer.RegisterType<BestAreaFitFreeRectangleExtractor>();
            unityContainer.RegisterType<DummyRectOrientationSelector>();
            unityContainer.RegisterType<GuillotineFreeRectangleSortedMerger>();
            unityContainer.RegisterType<LongerAxisGuillotineFreeRectangleSplitter>();

            //Register the skyline algorithm
            unityContainer.RegisterType<GuillotinePlacementAlgorithm>();

            var eventAggregator = unityContainer.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<ModulesLoadedEvent>().Subscribe(OnModulesLoaded);
        }

        /// <summary>
        /// Event handler of the <see cref="ModulesLoadedEvent"/>
        /// Gets all the Guillotine algorithm's dependencies that were loaded by the PaunPacker.GUI and passes them
        /// to the <see cref="GuillotineAlgorithmViewModel"/> so that the user could select it via the view of this plugin
        /// </summary>
        /// <param name="payload">The payload of the event</param>
        private void OnModulesLoaded(ModulesLoadedPayload payload)
        {
            var view = new GuillotineAlgorithmView()
            {
                DataContext = new GuillotineAlgorithmViewModel(
                    payload.GetLoadedTypes<IFreeRectangleExtractor>()
                        .Concat(unityContainer.Registrations.Select(x => x.RegisteredType).Where(x => typeof(IFreeRectangleExtractor).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface)),
                    payload.GetLoadedTypes<IFreeRectangleSplitter>()
                        .Concat(unityContainer.Registrations.Select(x => x.RegisteredType).Where(x => typeof(IFreeRectangleSplitter).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface)),
                    payload.GetLoadedTypes<IFreeRectangleMerger>()
                        .Concat(unityContainer.Registrations.Select(x => x.RegisteredType).Where(x => typeof(IFreeRectangleMerger).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface)),
                    payload.GetLoadedTypes<IRectOrientationSelector>()
                        .Concat(unityContainer.Registrations.Select(x => x.RegisteredType).Where(x => typeof(IRectOrientationSelector).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface)),
                    payload.GetLoadedTypes<IFreeRectanglePostProcessor>()
                        .Concat(unityContainer.Registrations.Select(x => x.RegisteredType).Where(x => typeof(IFreeRectanglePostProcessor).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface))
                    .Distinct(), unityContainer)
            };
            unityContainer.RegisterInstance<UserControl>(PluginViewWiring.GetViewName(typeof(GuillotinePlacementAlgorithm)), view);
        }

        /// <summary>
        /// Does not do anything
        /// Needed only to implement the interface
        /// </summary>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        private IUnityContainer unityContainer;
    }
}
