using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Windows;
using MoreLinq.Extensions;
using PaunPacker.Core.ImageProcessing;
using PaunPacker.Core.Metadata;
using PaunPacker.Core.Packing.MBBF;
using PaunPacker.Core.Packing.Placement;
using PaunPacker.Core.Packing.Sorting;
using PaunPacker.GUI.Dialogs;
using PaunPacker.GUI.ViewModels;
using PaunPacker.GUI.Views;
using PaunPacker.GUI.Workarounds;
using PaunPacker.GUI.WPF.Common;
using PaunPacker.GUI.WPF.Common.Attributes;
using PaunPacker.GUI.WPF.Common.Events;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Services.Dialogs;
using Unity;

namespace PaunPacker.GUI
{
    /// <summary>
    /// This class represents the application
    /// </summary>
    public sealed class App : PrismApplication
    {
        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.8.0.0")]
        public static void Main()
        {
            if (File.Exists("SplashScreen.png"))
            {
                SplashScreen splashScreen = new SplashScreen("SplashScreen.png");
                splashScreen.Show(true);
            }

            using (var app = new PaunPacker.GUI.App())
            {
                app.Run();
            }
        }

        /// <summary>
        /// Constructs the App
        /// </summary>
        public App(): base(new UnityContainer())
        {
        }
        
        /// <summary>
        /// Implementation of PrismApplication's CreateShell method
        /// Creates a MainWindow it's View model and sets the DataContext to the created view model
        /// </summary>
        /// <returns>The main window</returns>
        protected override Window CreateShell()
        {
            var shell = new MainWindow();
            var mainWindowVM = new MainWindowViewModel(UnityContainer);
            mainWindowVM.ShouldClose += MainWindowVM_ShouldClose;
            shell.DataContext = mainWindowVM;
            return shell;
        }

        /// <summary>
        /// Event handler for the Main window's ShouldClose event, closes the MainWindow
        /// </summary>
        private void MainWindowVM_ShouldClose()
        {
            MainWindow.Close();
        }

        /// <summary>
        /// Implementation of PrismApplication's RegisterTypes method
        /// </summary>
        /// <param name="containerRegistry">The container registr</param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IRegionManager>(new RegionManager());
            containerRegistry.RegisterDialog<MessageDialogView, MessageDialogViewModel>();
            containerRegistry.RegisterDialog<LoadedPluginsView, LoadedPluginsViewModel>();
            containerRegistry.RegisterDialog<PluginView, PluginViewModel>();
        }

        //Based on https://github.com/PrismLibrary/Prism/issues/1732
        /// <summary>
        /// Creates the module catalog from and loads the plugins from a plugin folder
        /// It also loads the plugins using the MEF
        /// </summary>
        /// <remarks>This is also somehow a workaround because Prism's DirectoryModule is not supported on .NET Core
        /// so the plugins from a folder has to be loaded manually</remarks>
        /// <returns>The module catalog</returns>
        protected override IModuleCatalog CreateModuleCatalog()
        {
            AssemblyLoadContext ctx = AssemblyLoadContext.GetLoadContext(this.GetType().Assembly);

            DirectoryInfo directory;
            IEnumerable<FileInfo> fileInfos;

            try
            {
                directory = new DirectoryInfo(GUI.Resources.PluginFolder);
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
            var tmp = unityContainer.Registrations.Select(x => x.RegisteredType).Where(x => typeof(T).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface);
            if (attributeType != null)
            {
                tmp = tmp.Where(x => Attribute.IsDefined(x, attributeType));
            }
            return tmp;
        }

        /// <summary>
        /// For a given exported type <paramref name="exportedType"/> creates a <see cref="ExportedTypeViewModel"/>
        /// </summary>
        /// <param name="exportedType">The exported type</param>
        /// <param name="typeNameToAttributeMapping">Maps the Type's FullNames to <see cref="ExportedTypeMetadataAttribute"/></param>
        /// <returns>The <see cref="ExportedTypeViewModel"/> for a given exported type <paramref name="exportedType"/></returns>
        private ExportedTypeViewModel CreateExportedTypeViewModel(Type exportedType, Dictionary<string, ExportedTypeMetadataAttribute> typeNameToAttributeMapping)
        {
            var exportedTypeMetadata = exportedType.GetCustomAttributes<ExportedTypeMetadataAttribute>().FirstOrDefault();
            if (exportedTypeMetadata == null)
            {
                typeNameToAttributeMapping.TryGetValue(exportedType.FullName, out exportedTypeMetadata);
            }
            return new ExportedTypeViewModel(exportedType, exportedTypeMetadata);
        }


        /// <summary>
        /// Initializes the modules, imports all the types that are exported from the plugins and passes them to the <see cref="MainWindowViewModel"/>
        /// </summary>
        protected override void InitializeModules()
        {
            try
            {
                base.InitializeModules();
            }
            catch (ModularityException e)
            {
                //Error during initialization of some plugin
                //Be verbose so that the user could spot the broken plugin and remove it, for example ..
                var dialogService = Container.Resolve<IDialogService>();
                var dialogParams = new DialogParameters
                {
                    { MessageDialogParameterNames.Message, e.Message },
                    { MessageDialogParameterNames.Title, "Error" }
                };
                dialogService.ShowDialog("MessageDialogView", dialogParams, (dialogResult) =>
                {
                });
            }

            if (!loadedPlugins.Any(x => x.Assembly.FullName.StartsWith("PaunPacker.Plugins.DefaultImplementationsProviderPlugin,", StringComparison.InvariantCulture)))
            {
                //The default implementations plugin was not loaded
                var dialogService = Container.Resolve<IDialogService>();
                var dialogParams = new DialogParameters
                {
                    { MessageDialogParameterNames.Message, "Critical error was encountered, the PaunPacker could no run without the plugin with default implementations !" },
                    { MessageDialogParameterNames.Title, "Error" }
                };
                dialogService.ShowDialog("MessageDialogView", dialogParams, (dialogResult) =>
                {
                });
                Shutdown(-1);
            }

            //Assign modules to Shell's ViewModel properly ..
            var mainWindowVM = (MainWindowViewModel)MainWindow.DataContext;

            //Get All instances from mefContainer
            //The reason why only mefContainer is considered here is because unityContainer should be used mainly for
            //factoryRegistrations and dependencies used by these factories are not registered yet
            //On the other hand, MEF is used for easy&fast way to export dependency-less plugins so it is safe to instantiate them at any moment
            var imageSorters = mefContainer.GetExports<IImageSorter>();
            var placementAlgorithms = mefContainer.GetExports<IPlacementAlgorithm>();
            var minimumBoundingBoxFinders = mefContainer.GetExports<IMinimumBoundingBoxFinder>();
            var metadataWriters = mefContainer.GetExports<IMetadataWriter>();
            var imageProcessors = mefContainer.GetExports<IImageProcessor>();

            //Get All exported types (from both mefContainer & unityContainer)
            var imageSorterTypes = imageSorters.Select(x => x.GetType())
                .Concat(FilterUnityExportedTypesByTypeAndAttribute<IImageSorter>(UnityContainer));
            var placementAlgorithmTypes = placementAlgorithms.Select(x => x.GetType())
                .Concat(FilterUnityExportedTypesByTypeAndAttribute<IPlacementAlgorithm>(UnityContainer));
            var minimumBoundingBoxFinderTypes = minimumBoundingBoxFinders.Select(x => x.GetType())
                .Concat(FilterUnityExportedTypesByTypeAndAttribute<IMinimumBoundingBoxFinder>(UnityContainer));
            var metadataWriterTypes = metadataWriters.Select(x => x.GetType())
                .Concat(FilterUnityExportedTypesByTypeAndAttribute<IMetadataWriter>(UnityContainer));
            var imageProcessorTypes = imageProcessors.Select(x => x.GetType())
                .Concat(FilterUnityExportedTypesByTypeAndAttribute<IImageProcessor>(UnityContainer));

            mainWindowVM.LoadedPlugins = loadedPlugins
                .Concat(mefContainer.GetExports<IMetadataWriter>().Select(x => x.GetType()))
                .Concat(mefContainer.GetExports<IImageProcessor>().Select(x => x.GetType()))
                .Concat(mefContainer.GetExports<IImageSorter>().Select(x => x.GetType()))
                .Concat(mefContainer.GetExports<IPlacementAlgorithm>().Select(x => x.GetType()))
                .Concat(mefContainer.GetExports<IMinimumBoundingBoxFinder>().Select(x => x.GetType()))
                .Distinct();

            //ExportedTypeMetadataAttribute can be either placed directly on the exported type or on the Plugin Entry point type
            //Obtain all the ExportedTypeMetadataAttributes that are on the Plugin entry points instead of on the actual class being exported
            var exportedMetadataAttributesOnPlugins = mainWindowVM.LoadedPlugins
                .Select(x => x.GetCustomAttributes<ExportedTypeMetadataAttribute>());

            var exportedTypeNameToExportedTypeAttributeMapping = new Dictionary<string, ExportedTypeMetadataAttribute>();
            foreach (var plugin in exportedMetadataAttributesOnPlugins)
            {
                foreach (var exportedType in plugin)
                {
                    exportedTypeNameToExportedTypeAttributeMapping[exportedType.ExportedType.FullName] = exportedType;
                }
            }

            mainWindowVM.ImageSorterVMs = imageSorterTypes
                .Distinct()
                .Select(x => CreateExportedTypeViewModel(x, exportedTypeNameToExportedTypeAttributeMapping));
            mainWindowVM.PlacementAlgorithmVMs = placementAlgorithmTypes
                .Distinct()
                .Select(x => CreateExportedTypeViewModel(x, exportedTypeNameToExportedTypeAttributeMapping));
            mainWindowVM.MinimumBoundingBoxFinderVMs = minimumBoundingBoxFinderTypes
                .Distinct()
                .Select(x => CreateExportedTypeViewModel(x, exportedTypeNameToExportedTypeAttributeMapping));
            mainWindowVM.MetadataWriterVMs = metadataWriterTypes
                .Distinct()
                .Select(x => CreateExportedTypeViewModel(x, exportedTypeNameToExportedTypeAttributeMapping));
            mainWindowVM.ImageProcessorVMs = imageProcessorTypes
                .Distinct()
                .Select(x => CreateExportedTypeViewModel(x, exportedTypeNameToExportedTypeAttributeMapping));
            
            //The event about modules being loaded is published before view manipulation (because modules could register view in event handler of the modules loaded event)
            var eventAggregator = UnityContainer.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<ModulesLoadedEvent>().Publish(new ModulesLoadedPayload(
                imageSorterTypes
                .Concat(placementAlgorithmTypes)
                .Concat(minimumBoundingBoxFinderTypes)
                .Concat(metadataWriterTypes)
                .Concat(imageProcessorTypes)
            ));

            var regionManager = Container.Resolve<IRegionManager>();

            //Get views for ImageSorters
            var collection = regionManager.Regions[RegionNames.ImageSortersRegion];
            foreach (var imageSorter in mainWindowVM.ImageSorterVMs)
            {
                var view = UnityContainer.Resolve<System.Windows.Controls.UserControl>(PluginViewWiring.GetViewName(imageSorter.ExportedType));
                if (view != null)
                {
                    regionManager.Regions[RegionNames.ImageSortersRegion].Add(view, PluginViewWiring.GetViewName(imageSorter.ExportedType));
                    collection.Deactivate(view);
                }
            }

            //Get views for PlacementAlgorithms, register them and hide all of them
            collection = regionManager.Regions[RegionNames.PlacementAlgorithmsRegion];
            foreach (var placementAlgorithm in mainWindowVM.PlacementAlgorithmVMs)
            {
                var view = UnityContainer.Resolve<System.Windows.Controls.UserControl>(PluginViewWiring.GetViewName(placementAlgorithm.ExportedType));
                if (view != null)
                {
                    regionManager.Regions[RegionNames.PlacementAlgorithmsRegion].Add(view, PluginViewWiring.GetViewName(placementAlgorithm.ExportedType));
                    collection.Deactivate(view);
                }
            }

            //Get views for MinimumBoundingBoxFinders
            collection = regionManager.Regions[RegionNames.MinimumBoundingBoxFinderRegion];
            foreach (var minBoundingBoxFinder in mainWindowVM.MinimumBoundingBoxFinderVMs)
            {
                var view = UnityContainer.Resolve<System.Windows.Controls.UserControl>(PluginViewWiring.GetViewName(minBoundingBoxFinder.ExportedType));
                if (view != null)
                {
                    regionManager.Regions[RegionNames.MinimumBoundingBoxFinderRegion].Add(view, PluginViewWiring.GetViewName(minBoundingBoxFinder.ExportedType));
                    collection.Deactivate(view);
                }
            }


            //Get views for MetadataWriters
            collection = regionManager.Regions[RegionNames.MetadataWritersRegion];
            foreach (var metadataWriter in mainWindowVM.MetadataWriterVMs)
            {
                var view = UnityContainer.Resolve<System.Windows.Controls.UserControl>(PluginViewWiring.GetViewName(metadataWriter.ExportedType));
                if (view != null)
                {
                    regionManager.Regions[RegionNames.MetadataWritersRegion].Add(view, PluginViewWiring.GetViewName(metadataWriter.ExportedType));
                    collection.Deactivate(view);
                }
            }

            //Get views for ImageProcessors
            collection = regionManager.Regions[RegionNames.ImageProcessorsRegion];
            foreach (var imageProcessor in mainWindowVM.ImageProcessorVMs)
            {
                var view = UnityContainer.Resolve<System.Windows.Controls.UserControl>(PluginViewWiring.GetViewName(imageProcessor.ExportedType));
                if (view != null)
                {
                    regionManager.Regions[RegionNames.ImageProcessorsRegion].Add(view, PluginViewWiring.GetViewName(imageProcessor.ExportedType));
                    collection.Deactivate(view);
                }
            }

            mainWindowVM.Initialize();
        }

        /// <summary>
        /// Implementation of the <see cref="IDisposable"/>
        /// </summary>
        /// <param name="disposing">Indicates whether this object is already being disposed in order to prevent recursive disposal</param>
        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                STAThreadTaskScheduler.Scheduler.Dispose();
                mefContainer.Dispose();
            }

            disposed = true;
            base.Dispose(disposing);
        }

        private bool disposed;
        private IModuleCatalog moduleCatalog;
        private IEnumerable<Type> loadedPlugins;
        private CompositionHost mefContainer;
    }

}

