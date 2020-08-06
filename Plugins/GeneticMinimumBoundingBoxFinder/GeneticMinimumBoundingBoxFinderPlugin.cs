using System.Windows.Controls;
using PaunPacker.GUI.WPF.Common;
using PaunPacker.GUI.WPF.Common.Attributes;
using Prism.Ioc;
using Prism.Modularity;
using Unity;

namespace PaunPacker.Plugins
{
    /// <summary>
    /// Plugin entrypoint that exports the <see cref="GeneticMinimumBoundingBoxFinder"/>
    /// </summary>
    [PluginMetadata(typeof(GeneticMinimumBoundingBoxFinderPlugin), nameof(GeneticMinimumBoundingBoxFinderPlugin), "Plugin containing GeneticMinimumBoundingBoxFinder", "PaunPacker", "1.0.0.0", typeof(GeneticMinimumBoundingBoxFinder))]
    public class GeneticMinimumBoundingBoxFinderPlugin : IModule
    {
        /// <summary>
        /// Creates the view and the view model
        /// The view is then registered so that the PaunPacker.GUI could load it later
        /// </summary>
        /// <remarks>Implementation of the Prism's <see cref="IModule"/> interface</remarks>
        /// <param name="containerProvider">Container provider given by the Prism</param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
            IUnityContainer container = containerProvider.Resolve<IUnityContainer>();
            var view = new Views.GeneticMinimumBoundingBoxFinderView
            {
                DataContext = new ViewModels.GeneticMinimumBoundingBoxFinderViewModel(container)
            };
            container.RegisterInstance<UserControl>(PluginViewWiring.GetViewName(typeof(GeneticMinimumBoundingBoxFinder)), view);
        }

        /// <summary>
        /// Does not do anything
        /// Needed only to implement the interface
        /// </summary>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}
