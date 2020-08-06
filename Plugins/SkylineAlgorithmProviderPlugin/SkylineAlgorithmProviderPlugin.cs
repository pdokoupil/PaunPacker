using System.Linq;
using System.Windows.Controls;
using PaunPacker.Core.Packing.Placement.Skyline;
using PaunPacker.GUI.WPF.Common;
using PaunPacker.GUI.WPF.Common.Attributes;
using PaunPacker.GUI.WPF.Common.Events;
using PaunPacker.Plugins.ViewModels;
using PaunPacker.Plugins.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Unity;

namespace PaunPacker.Plugins
{
    /// <summary>
    /// Main entry point of the plugin.
    /// This plugin exports the <see cref="SkylineAlgorithm"/> and provides a GUI for it
    /// This GUI was seperated from PaunPacker.Core and PaunPacker.GUI to separate Plugin
    /// </summary>
    [PluginMetadata(typeof(SkylineAlgorithmProviderPlugin), nameof(SkylineAlgorithmProviderPlugin), "Plugin exporting SkylineAlgorithm and several RectAndPointPickers. It also exports a view that allows the user to select the RectAndPoint picker to be used.",
        "PaunPacker",
        "1.0.0.0",
        typeof(SkylineAlgorithm), typeof(LightweightRectAndPointPicker), typeof(MinimalAreaWasteRectAndPointPicker))]
    [ExportedTypeMetadata(typeof(SkylineAlgorithm), nameof(SkylineAlgorithm), "Implementation of Skyline Algorithm", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(LightweightRectAndPointPicker), nameof(LightweightRectAndPointPicker), "Simple & Lightweight implementation RectAndPointPicker", "PaunPacker", "1.0.0.0")]
    [ExportedTypeMetadata(typeof(MinimalAreaWasteRectAndPointPicker), nameof(MinimalAreaWasteRectAndPointPicker), "Implementation of RectAndPointPicker that selects based on minimal area waste", "PaunPacker", "1.0.0.0")]
    public class SkylineAlgorithmProviderPlugin : IModule
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

            //Register known RectAndPointPickers
            unityContainer.RegisterType<LightweightRectAndPointPicker>();
            unityContainer.RegisterType<MinimalAreaWasteRectAndPointPicker>();

            //Register the skyline algorithm
            unityContainer.RegisterType<SkylineAlgorithm>();

            //Register the OnModulesLoaded event
            var eventAggregator = unityContainer.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<ModulesLoadedEvent>().Subscribe(OnModulesLoaded);
        }

        /// <summary>
        /// Event handler of the <see cref="ModulesLoadedEvent"/>
        /// Gets all the <see cref="ISkylineRectAndPointPicker"/> that were loaded by the PaunPacker.GUI and passes them
        /// to the <see cref="SkylineAlgorithmViewModel"/> so that the user could select it via the view of this plugin
        /// </summary>
        /// <param name="payload">The payload of the event</param>
        private void OnModulesLoaded (ModulesLoadedPayload payload)
        {
            var view = new SkylineAlgorithmView()
            {
                DataContext = new SkylineAlgorithmViewModel(payload.GetLoadedTypes<ISkylineRectAndPointPicker>()
                    .Concat(unityContainer.Registrations.Select(x => x.RegisteredType).Where(x => typeof(ISkylineRectAndPointPicker).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface))
                    .Distinct(), unityContainer)
            };
            unityContainer.RegisterInstance<UserControl>(PluginViewWiring.GetViewName(typeof(SkylineAlgorithm)), view);
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
