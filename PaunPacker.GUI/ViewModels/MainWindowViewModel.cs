using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MoreLinq.Extensions;
using PaunPacker.Core.Atlas;
using PaunPacker.Core.ImageProcessing;
using PaunPacker.Core.Metadata;
using PaunPacker.Core.Packing;
using PaunPacker.Core.Packing.MBBF;
using PaunPacker.Core.Packing.Placement;
using PaunPacker.Core.Packing.Sorting;
using PaunPacker.Core.Types;
using PaunPacker.GUI.Dialogs;
using PaunPacker.GUI.Events;
using PaunPacker.GUI.Views;
using PaunPacker.GUI.WPF.Common;
using PaunPacker.GUI.WPF.Common.Attributes;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using SkiaSharp;
using Unity;

namespace PaunPacker.GUI.ViewModels
{
    /// <summary>
    /// View model for a main window
    /// </summary>
    sealed class MainWindowViewModel : ViewModelBase, IDisposable
    {
        /// <summary>
        /// Constructs the view model
        /// Initializes the commands and other members
        /// </summary>
        /// <param name="unityContainer">Unity IoC container</param>
        public MainWindowViewModel(IUnityContainer unityContainer)
        {
            //Default member initialization
            eventAggregator = unityContainer.Resolve<IEventAggregator>();
            regionManager = unityContainer.Resolve<IRegionManager>();
            dialogService = unityContainer.Resolve<IDialogService>();

            SelectedRectangleVMs = Enumerable.Empty<RectangleViewModel>();

            eventAggregator.GetEvent<RectanglesSelectedEvent>().Subscribe(payload =>
            {
                SelectedRectangleVMs = payload.SelectedRectangles.Select(rectangle => new RectangleViewModel(rectangle));
            });

            //Set-up default Placement, Sorter, BoundingBoxFinder ...
            IUnityContainer container = unityContainer;
            //childContainer is needed for the case where Both PlacementAlgorithm & MinimumBoundingBoxFinder take IImageSorter (and want possibly different kind of it ..)
            IUnityContainer childContainer = container.CreateChildContainer();

            IoC = container;

            container.RegisterFactory<IImageSorter>((c) =>
            {
                return c.Resolve(PlacementImageSorterVM.ExportedType);
            });

            childContainer.RegisterFactory<IImageSorter>((c) =>
            {
                return c.Resolve(ImageSorterVM.ExportedType);
            });

            container.RegisterFactory<IPlacementAlgorithm>((_) =>
            {
                return ResolveTypeAs<IPlacementAlgorithm>(container, PlacementAlgorithmVM.ExportedType);
            });

            container.RegisterFactory<FixedSizePacker>((_) =>
            {
                return new FixedSizePacker(FixedWidth, FixedHeight, ResolveTypeAs<IPlacementAlgorithm>(container, PlacementAlgorithmVM.ExportedType));
            });

            container.RegisterFactory<IMinimumBoundingBoxFinder>((cont, type, name) =>
            {
                return ResolveTypeAs<IMinimumBoundingBoxFinder>(childContainer, MinimumBoundingBoxFinderVM.ExportedType);
            });

            container.RegisterFactory<IImageProcessor>((_) =>
            {
                return ResolveTypeAs<IImageProcessor>(container, ImageProcessorVM.ExportedType);
            });

            container.RegisterFactory<IMetadataWriter>((_) =>
            {
                return ResolveTypeAs<IMetadataWriter>(container, MetadataWriterVM.ExportedType);
            });

            LoadFile = new RelayCommand((_) =>
            {
                var service = new Services.OpenFileService(Common.FileFilters.ImageExtensionFilter);
                var files = service.GetFiles();
                if (files != null)
                {
                    CurrentJob = new Job("Loading a file(s)");
                    CurrentJob.StartJobAsync(() =>
                    {
                        return files.Select(x =>
                        {
                            var img = BitmapManager.LoadBitmap(x);
                            if (img == null)
                            {
                                return null;
                            }
                            return new RectangleViewModel(new PPRect(img));
                        }).Union(AllRectsVM?.AllRectangles ?? Enumerable.Empty<RectangleViewModel>()).Where(x => x != null).DistinctBy(x => x.Rectangle.Image.ImagePath);

                    }).ContinueWith(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                        {
                            var rectangleVMs = task.Result;
                            AllRectsVM = new AllRectanglesViewModel(rectangleVMs); //load files now do not reload previous..
                            LoadedImagesTreeVM = new LoadedImagesTreeViewModel(AllRectsVM.Rectangles);
                        }
                        CurrentJob = null;
                        IsJobCancelling = false;
                    }, STAThreadTaskScheduler.Scheduler).ConfigureAwait(true);
                }

            }, (_) => true);

            GenerateTextureAtlas = new RelayCommand(async (_) =>
            {
                if (AllRectsVM == null || AllRectsVM.AllRectangles.Count == 0)
                {
                    ShowWarning("You don't have loaded any images !");
                    return;
                }

                MinimumBoundingBoxFinder = IoC.Resolve<IMinimumBoundingBoxFinder>();
                
                PackingResult textureAtlasToSave = null, textureAtlasToShow = null;

                TextureAtlasVM = null; 
                cancellationTokenSource = new CancellationTokenSource();


                CurrentJob = new Job("Generating");
                textureAtlasToSave = await CurrentJob.StartJobAsync(() =>
                {
                    textureAtlasToSave = textureAtlasToShow = MinimumBoundingBoxFinder.FindMinimumBoundingBox(AllRectsVM.Rectangles, cancellationTokenSource.Token);
                    //Handle the alias creation
                    if (IsAliasCreationEnabled)
                    {
                        var tst = AllRectsVM.RectanglesByHash.SelectMany(group => group);
                        var aliasedRectanglesVMs = AllRectsVM.RectanglesByHash
                            .Select(x => new PPRect(new PPImage(x.Select(y => y.Rectangle.Image).AsEnumerable())));

                        textureAtlasToSave = MinimumBoundingBoxFinder.FindMinimumBoundingBox(aliasedRectanglesVMs, cancellationTokenSource.Token);
                    }

                    return textureAtlasToSave;
                }, MinimumBoundingBoxFinder).ContinueWith(result =>
                {
                    IsJobCancelling = false;
                    CurrentJob = null;
                    return result.Result;
                }, STAThreadTaskScheduler.Scheduler)
                .ConfigureAwait(true);

                if (textureAtlasToShow != null)
                {
                    TextureAtlasVM = new TextureAtlasViewModel(new TextureAtlas(textureAtlasToShow), SelectedColorType);
                    var atlasToSave = new TextureAtlas(textureAtlasToSave);
                    //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TextureAtlasVM")); //or should be there?
                    if (SaveMetadataPath != null)
                    {
                        try
                        {
                            MetadataWriter = IoC.Resolve<IMetadataWriter>();
                        }
                        catch (ResolutionFailedException)
                        {
                            ShowError($"There is no metadata available for the selected target framework {SelectedTargetFramework}");
                            return;
                        }

                        cancellationTokenSource = new CancellationTokenSource();
                        CurrentJob = new Job("Generating Metadata");
                        await CurrentJob.StartJobAsync(async () =>
                        {
                            await MetadataWriter.WriteAsync(SaveMetadataPath, SaveAtlasPath ?? "<was not specified>", new MetadataCollection(atlasToSave), cancellationTokenSource.Token)
                                .ConfigureAwait(true);
                        }, MetadataWriter).ContinueWith(result =>
                        {
                            IsJobCancelling = false;
                            CurrentJob = null;
                        }, STAThreadTaskScheduler.Scheduler)
                        .ConfigureAwait(true);
                    }

                    if (SaveAtlasPath != null)
                    {
                        BitmapManager.SaveBitmap(atlasToSave.Image, SaveAtlasPath, SKEncodedImageFormat.Png, true); //format is hardcoded now
                        if (SaveMetadataPath != null && 
                            Path.GetDirectoryName(SaveAtlasPath) != Path.GetDirectoryName(SaveMetadataPath))
                        {
                            ShowWarning("An output folder for metadata is different from output folder of the texture atlas.\n Beware that some metadata formats require a texture atlas file to be placed in the same folder in order to reference it using relative path (e.g. libGDX)");
                        }
                    }
                    else if (SaveMetadataPath != null)
                    {
                        ShowWarning("Path for a texture atlas was not specified therefore only a metadata file was generated and stored.\n Beware that some metadata formats require a texture atlas file in order to reference it");
                    }
                }
                else
                {
                    ShowWarning("No feasible packing found !");
                }
            }, (_) => true);


            CancelGenerateAtlas = new RelayCommand((_) =>
            {
                IsJobCancelling = true;
                cancellationTokenSource.Cancel();
            }, (_) => true);

            NewProject = new RelayCommand((_) =>
            {
                var createProjectWindow = new CreateProjectWindow(dialogService);
                if (createProjectWindow.ShowDialog() == true)
                {
                    Project = createProjectWindow.CreatedProject;
                    if (Project != null)
                    {
                        Project.OpenedImages = AllRectsVM?.Rectangles.Select(x => x.Image);
                    }
                }
            }, (_) => true);

            OpenProject = new RelayCommand((_) =>
            {
                var service = new Services.OpenFileService(Common.FileFilters.ProjectExtensionFilter);
                var file = service.GetFile();
                string errorMessage = null;
                if (file != null)
                {
                    Project = ProjectManager.LoadProject(file, out errorMessage);
                    if (Project == null)
                    {
                        ShowError($"The project file {file} is corrupted. No project was loaded");
                    }
                    else
                    {
                        AllRectsVM = new AllRectanglesViewModel(Project.OpenedImages.Select(x => new RectangleViewModel(new PPRect(x))));
                        LoadedImagesTreeVM = new LoadedImagesTreeViewModel(AllRectsVM.Rectangles);
                    }
                }
            }, (_) => true);

            SaveProjectAs = new RelayCommand((_) =>
            {
                if (Project == null)
                {
                    return;
                }
                var service = new Services.SaveFileService(Common.FileFilters.ProjectExtensionFilter);
                var file = service.GetFile();
                if (file != null)
                {
                    Project.Path = file;
                    ProjectManager.SaveProject(Project, file);
                    ShowNotification("The project was successfully saved", "Success");
                }
            }, (_) => true);

            SaveProject = new RelayCommand((_) =>
            {
                if (Project != null)
                {
                    ProjectManager.SaveProject(Project);
                    ShowNotification("The project was successfully saved", "Success");
                }
            }, (_) => true);

            Exit = new RelayCommand((_) =>
            {
                ShouldClose?.Invoke();
            }, (_) => true); //return true only if no pending operation is running???

            CloseProject = new RelayCommand((_) =>
            {
                Project = null;
            }, (_) => true);

            LoadFolder = new RelayCommand((_) =>
            {
                var service = new Services.OpenFolderService();
                var folderPath = service.GetFolder();
                if (folderPath != null)
                {
                    CurrentJob = new Job("Loading a folder");
                    CurrentJob.StartJobAsync(() =>
                    {
                        var imagePaths = Common.IOUtilities.GetAllFilesRecursively(folderPath)
                                     .Where(file => Common.FileFilters.SupportedImageFormatExtensions.Contains(file.Extension))
                                     .Select(file => file.FullName);

                        return imagePaths.Select(x =>
                        {
                            var loadedImg = BitmapManager.LoadBitmap(x);
                            if (loadedImg == null)
                            {
                                return null;
                            }
                            return new RectangleViewModel(new PPRect(loadedImg));
                        }).Union(AllRectsVM?.AllRectangles ?? Enumerable.Empty<RectangleViewModel>()).Where(x => x != null).DistinctBy(x => x.Rectangle.Image.ImagePath);

                    }).ContinueWith(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                        {
                            var rectangleVMs = task.Result;
                            AllRectsVM = new AllRectanglesViewModel(rectangleVMs.ToArray()); //The ToArray() is needed there, otherwise if the texture atlas is stored in the same directory as the input directory, it gets packed with the rest
                                                                                             //of the textures again and again
                            LoadedImagesTreeVM = new LoadedImagesTreeViewModel(AllRectsVM.Rectangles); //merge instead
                        }
                        CurrentJob = null;
                        IsJobCancelling = false;
                    }, STAThreadTaskScheduler.Scheduler).ConfigureAwait(true);
                }
            }, (_) => true);

            SaveMetadata = new RelayCommand((_) =>
            {
                var service = new Services.SaveFileService(Common.FileFilters.MetadataExtensionFilter);
                var file = service.GetFile();
                SaveMetadataPath = file;
            }, (_) => true);

            SaveAtlas = new RelayCommand((_) =>
            {
                var service = new Services.SaveFileService(Common.FileFilters.AtlasExtensionFilter);
                var file = service.GetFile();
                SaveAtlasPath = file;
            }, (_) => true);

            SelectBoundaryMode = new RelayCommand((selectedBoundaryMode) =>
            {
                SelectedBoundaryModeString = (string)selectedBoundaryMode;
                
                if (SelectedBoundaryModeString == Resources.AutoBoundaryModeString)
                {
                    MinimumBoundingBoxFinderVM = MinimumBoundingBoxFinderVMs.FirstOrDefault(x => x.ExportedType == typeof(UnknownSizePacker));
                }
                else if (SelectedBoundaryModeString == Resources.FixedSizeBoundaryModeString)
                {
                    MinimumBoundingBoxFinderVM = MinimumBoundingBoxFinderVMs.FirstOrDefault(x => x.ExportedType == typeof(FixedSizePacker));
                }
                else if (SelectedBoundaryModeString == Resources.PowersOfTwoBoundaryModeString)
                {
                    MinimumBoundingBoxFinderVM = MinimumBoundingBoxFinderVMs.FirstOrDefault(x => x.ExportedType == typeof(PowerOfTwoSizePacker));
                }
                else
                {
                    throw new InvalidOperationException("This should never happen");
                }

                var selected = selectedBoundaryMode.ToString();
            }, (_) => true);


            ShowBorders = new RelayCommand((_) =>
            {
                if (TextureAtlasVM != null)
                {
                    TextureAtlasVM.AreBordersShown = true;
                }
            }, (_) => true);

            SelectAll = new RelayCommand((_) =>
            {
                if (TextureAtlasVM != null)
                {
                    TextureAtlasVM.SelectAllRectangles();
                }
            }, (_) => true);

            UnselectAll = new RelayCommand((_) =>
            {
                if (TextureAtlasVM != null)
                {
                    TextureAtlasVM.UnselectAllRectangles();
                }
            }, (_) => true);

            UnshowBorders = new RelayCommand((_) =>
            {
                if (TextureAtlasVM != null)
                {
                    TextureAtlasVM.AreBordersShown = false;
                }
            }, (_) => true);

            ApplyImageProcessor = new RelayCommand(async (_) =>
            {
                //Resolve selected image processor
                ImageProcessor = IoC.Resolve<IImageProcessor>(); //TODO: Add Progress bar?? call it in tasks etc ... ?

                if (!SelectedRectangleVMs.Any())
                {
                    return;
                }

                //Images are processed in parallel
                CurrentJob = new Job("Processing images");
                cancellationTokenSource = new CancellationTokenSource();
                await CurrentJob.StartJobAsync(() =>
                {
                    var tasks = SelectedRectangleVMs
                    .Select(rectangleVM =>
                    {
                        return Task.Run(() => ImageProcessor.ProcessImage(rectangleVM.Rectangle.Image, cancellationTokenSource.Token))
                        .ContinueWith(processedBitmap =>
                        {
                            return new RectangleViewModel(new PPRect(processedBitmap.Result));
                        }, TaskScheduler.Default);
                    });

                    return Task.WhenAll(tasks);
                }).ContinueWith(async task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        var r = await task.Result.ConfigureAwait(true);
                        AllRectsVM.UpdateRectangles(r);
                        TextureAtlasVM = null;
                        //Try to generate the texture atlas again (need to be executed from a thread with proper synch. context)
                        if (GenerateTextureAtlas.CanExecute(null))
                        {
                            GenerateTextureAtlas.Execute(null);
                        }
                    }
                    CurrentJob = null;
                    IsJobCancelling = false;
                }, TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(false); //We need current synchronization context because the texture atlas re-generation might show dialog which needs to run on a certain thread

            }, (_) => true);


            LoadedPluginsNotificationCommand = new RelayCommand((_) =>
            {
                var dialogParameters = new DialogParameters()
                {
                    { LoadedPluginsDialogParameterNames.Title, "Loaded Plugins" },
                    { LoadedPluginsDialogParameterNames.LoadedPlugins, LoadedPlugins?.Select(x => new PluginViewModel(x)) }
                };
                    dialogService.ShowDialog(DialogNames.LoadedPluginsDialog, dialogParameters, (_) =>
                {
                });
            }, (_) => true);

            About = new RelayCommand((_) => 
            {
                var dialogParameters = new DialogParameters()
                {
                    { MessageDialogParameterNames.Title, "About PaunPacker" },
                    { MessageDialogParameterNames.Message, @"
The PaunPacker is an extensible tool for creating texture atlases
The extensibility lies in the posibility to create and import plugins that are containing algorithms for packing, image procesing and
metadata exporting

Copyright (c) 2019 Patrik Dokoupil
" }
                };
                dialogService.ShowDialog(DialogNames.MessageDialog, dialogParameters, (_) =>
                {
                });
            }, (_) => true);
        }

        /// <summary>
        /// Shows error notification
        /// </summary>
        /// <param name="message">Message to be shown</param>
        /// <param name="title">Title of the notification</param>
        private void ShowError(string message, string title = "Error")
        {
            ShowNotification(message, title);
        }

        /// <summary>
        /// Shows warning notification
        /// </summary>
        /// <param name="message">Message to be shown</param>
        /// <param name="title">Title of the notification</param>
        private void ShowWarning(string message, string title = "Warning")
        {
            ShowNotification(message, title);
        }

        /// <summary>
        /// Shows a notification
        /// </summary>
        /// <param name="message">Message to be shown</param>
        /// <param name="title">Title of the notification</param>
        private void ShowNotification(string message, string title)
        {
            var parameters = new DialogParameters()
            {
                { MessageDialogParameterNames.Title, title},
                { MessageDialogParameterNames.Message, message }
            };
            dialogService.ShowDialog(DialogNames.MessageDialog, parameters, (x) => { });
        }

        /// <summary>
        /// Shows a notification dialog displaying the loaded plugins
        /// </summary>
        public ICommand LoadedPluginsNotificationCommand
        {
            get; private set;
        }
        
        /// <summary>
        /// Shows a Dialog with information about PaunPacker
        /// </summary>
        public ICommand About
        {
            get; private set;
        }

        /// <summary>
        /// Types of the loaded plugins
        /// </summary>
        public IEnumerable<Type> LoadedPlugins
        {
            get; set;
        }

        /// <summary>
        /// Initializes the default values for the <see cref="SelectedBoundaryModeString"/>, <see cref="SelectedPackingSettingsMode"/>, <see cref="MetadataWriterVM"/> and <see cref="SelectedColorType"/>
        /// </summary>
        /// <remarks>Should be called from <see cref="App"/> after modules are initialized</remarks>
        public void Initialize()
        {
            SelectedBoundaryModeString = Resources.AutoBoundaryModeString;
            SelectedPackingSettingsMode = Common.UIConstants.BASIC_MODE_INDEX;
            MetadataWriterVM = MetadataWriterVMs.FirstOrDefault();
            ImageSorterVM = ImageSorterVMs.FirstOrDefault();
            ImageProcessorVM = ImageProcessorVMs.FirstOrDefault();
            SelectedColorType = SKColorType.Rgba8888;
            ModulesInitialized = true;
        }

        /// <summary>
        /// Command handling a file load
        /// </summary>
        public ICommand LoadFile
        {
            get; private set;
        }

        /// <summary>
        /// Command handling a texture atlas generation
        /// </summary>
        public ICommand GenerateTextureAtlas
        {
            get; private set;
        }

        /// <summary>
        /// Command handling a new project creation
        /// </summary>
        public ICommand NewProject
        {
            get; private set;
        }

        /// <summary>
        /// Command handling an openning of a project
        /// </summary>
        public ICommand OpenProject
        {
            get; private set;
        }

        /// <summary>
        /// Command handling a closing of a project
        /// </summary>
        public ICommand CloseProject
        {
            get; private set;
        }

        /// <summary>
        /// Command handling a project SaveAs
        /// </summary>
        public ICommand SaveProjectAs
        {
            get; private set;
        }

        /// <summary>
        /// Command handling a project Save
        /// </summary>
        public ICommand SaveProject
        {
            get; private set;
        }

        /// <summary>
        /// Command handling an exit of the application
        /// </summary>
        public ICommand Exit
        {
            get; private set;
        }

        /// <summary>
        /// The current project
        /// </summary>
        public Project Project
        {
            get => project;
            private set
            {
                this.project = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Project)));
            }
        }

        /// <summary>
        /// Command handling a load of a folder
        /// </summary>
        public ICommand LoadFolder
        {
            get; private set;
        }

        /// <summary>
        /// Command handling metadata saving
        /// </summary>
        public ICommand SaveMetadata
        {
            get; private set;
        }

        /// <summary>
        /// Command handling texture atlas saving
        /// </summary>
        public ICommand SaveAtlas
        {
            get; private set;
        }

        /// <summary>
        /// Command handling a cancellation of a texture atlas generation
        /// </summary>
        public ICommand CancelGenerateAtlas
        {
            get; private set;
        }

        /// <summary>
        /// Command handling a change in selected boundary mode
        /// </summary>
        public ICommand SelectBoundaryMode
        {
            get; private set;
        }

        /// <summary>
        /// Command handling a show of the borders
        /// </summary>
        public ICommand ShowBorders
        {
            get; private set;
        }

        /// <summary>
        /// Command handling Select all rectangles action
        /// </summary>
        public ICommand SelectAll
        {
            get; private set;
        }

        /// <summary>
        /// Command handling Unselect all rectangles action
        /// </summary>
        public ICommand UnselectAll
        {
            get; private set;
        }

        /// <summary>
        /// Command handling Unshow borders action
        /// </summary>
        public ICommand UnshowBorders
        {
            get; private set;
        }

        /// <summary>
        /// Command handling an application of a selected image processor
        /// </summary>
        /// <remarks>
        /// The images are processed in parallel by a single selected image processor
        /// That is the reason why the image processors does not report progress
        /// If the images were process sequentially then progress report would be possible
        /// </remarks>
        public ICommand ApplyImageProcessor
        {
            get; private set;
        }

        /// <summary>
        /// The view model of a texture atlas
        /// </summary>
        public TextureAtlasViewModel TextureAtlasVM
        {
            get => textureAtlasVM;
            private set
            {
                textureAtlasVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextureAtlasVM)));
            }
        }

        /// <summary>
        /// The view model of all the rectangles
        /// </summary>
        public AllRectanglesViewModel AllRectsVM
        {
            get => allRectsVM;
            private set
            {
                allRectsVM = value;
                if (Project != null)
                {
                    //Notify the project about changes in the loaded images
                    Project.OpenedImages = value.AllRectangles.Select(x => x.Rectangle.Image);
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AllRectsVM)));
            }
        }

        /// <summary>
        /// The view model for a tree view with all the loaded images
        /// </summary>
        public LoadedImagesTreeViewModel LoadedImagesTreeVM
        {
            get => loadedImagesTreeVM;
            private set
            {
                loadedImagesTreeVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadedImagesTreeVM)));
            }
        }

        /// <summary>
        /// The path where the texture atlas should be saved
        /// </summary>
        public string SaveAtlasPath
        {
            get => saveAtlasPath;
            set
            {
                saveAtlasPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveAtlasPath)));
            }
        }

        /// <summary>
        /// The path where the metadata should be saved
        /// </summary>
        public string SaveMetadataPath {
            get => saveMetadataPath;
            set
            {
                saveMetadataPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveMetadataPath)));
            }
        }

        /// <summary>
        /// The currently executing Job (texture atlas generation, image processin, etc.)
        /// </summary>
        public Job CurrentJob
        {
            get => currentJob;
            private set
            {
                currentJob = value;
                IsCurrentJobCancellable = value?.IsCancellable ?? false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentJob)));
            }
        }

        /// <summary>
        /// Determines whether the current job is being cancelled (i.e. that the cancellation was requested)
        /// </summary>
        public bool IsJobCancelling
        {
            get => isJobCancelling;
            private set
            {
                isJobCancelling = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsJobCancelling)));
            }
        }

        /// <summary>
        /// Whether the current job is cancellable or not
        /// </summary>
        public bool IsCurrentJobCancellable
        {
            get => isCurrentJobCancellable;
            private set
            {
                isCurrentJobCancellable = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCurrentJobCancellable)));
            }
        }

        /// <summary>
        /// Indicates whether the modules were initialized
        /// </summary>
        public bool ModulesInitialized
        {
            get; set;
        }

        /// <summary>
        /// Determines if the alias creation is enabled
        /// </summary>
        public bool IsAliasCreationEnabled
        {
            get => isAliasCreationEnabled;
            set
            {
                isAliasCreationEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAliasCreationEnabled)));
            }
        }

        /// <summary>
        /// Type of the selected Placement Algorithm
        /// </summary>
        public ExportedTypeViewModel PlacementAlgorithmVM
        {
            get => placementAlgorithmVM;
            set
            {
                var previousPlacementAlgorithmVM = placementAlgorithmVM;
                placementAlgorithmVM = value;

                if (value != null)
                {
                    if (Attribute.IsDefined(placementAlgorithmVM.ExportedType, typeof(SelfContainedAttribute)))
                    {
                        SelectedPlacementTakesSorter = false;
                    }
                    else
                    {
                        SelectedPlacementTakesSorter = ConstructorTakesExactlyGivenParameters(placementAlgorithmVM.ExportedType, typeof(IImageSorter));
                    }

                    if (ModulesInitialized)
                    {
                        var previousView = FindCorrespondingView(RegionNames.PlacementAlgorithmsRegion, previousPlacementAlgorithmVM.ExportedType);
                        HideView(RegionNames.PlacementAlgorithmsRegion, previousView);
                        ShowView(RegionNames.PlacementAlgorithmsRegion, FindCorrespondingView(RegionNames.PlacementAlgorithmsRegion, placementAlgorithmVM.ExportedType));
                    }
                }
                else
                {
                    SelectedPlacementTakesSorter = false;
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlacementAlgorithmVM)));
            }
        }

        /// <summary>
        /// Selected MinimumBoundingBoxFinder
        /// </summary>
        public IMinimumBoundingBoxFinder MinimumBoundingBoxFinder
        {
            get => minimumBoundingBoxFinder;
            set
            {
                minimumBoundingBoxFinder = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MinimumBoundingBoxFinder)));
            }
        }

        /// <summary>
        /// Type of the selected MinimumBoundingBoxFinder
        /// </summary>
        public ExportedTypeViewModel MinimumBoundingBoxFinderVM
        {
            get => minimumBoundingBoxFinderVM;
            set
            {
                var previousMinimumBoundingBoxFinderVM = minimumBoundingBoxFinderVM;
                minimumBoundingBoxFinderVM = value;

                if (value != null)
                {
                    if (Attribute.IsDefined(minimumBoundingBoxFinderVM.ExportedType, typeof(SelfContainedAttribute)))
                    {
                        SelectedPackerTakesPlacementAlgo = SelectedPackerTakesSorter = false;
                    }
                    else if (Attribute.IsDefined(minimumBoundingBoxFinderVM.ExportedType, typeof(PartiallyContainedAttribute)))
                    {
                        SelectedPackerTakesSorter = ConstructorContainsGivenParameters(minimumBoundingBoxFinderVM.ExportedType, typeof(IImageSorter));
                        SelectedPackerTakesPlacementAlgo = ConstructorContainsGivenParameters(minimumBoundingBoxFinderVM.ExportedType, typeof(IPlacementAlgorithm));
                    }
                    else
                    {
                        //Or just one of them
                        SelectedPackerTakesPlacementAlgo = ConstructorTakesExactlyGivenParameters(minimumBoundingBoxFinderVM.ExportedType, typeof(IPlacementAlgorithm));
                        SelectedPackerTakesSorter = ConstructorTakesExactlyGivenParameters(minimumBoundingBoxFinderVM.ExportedType, typeof(IImageSorter));

                        //If it only takes PlacementAlgo, there is still change that selected placement algo will take Sorter .. that case is handled in PlacementAlgorithm property setter ..
                        //the case where there is MinimumBoundingBoxFinder that takes both, and then user selects Placement that also takes sorter, both sorters are set to be same ..
                        //but this behavior should be avoided - creator of MinimumBoundingBoxFinder, should either provide ctor with PlacementAlgorithm, with no parameters, or provide placement algorithm with VIEW (and complete plugin) and construct it in it.. (and export fully created instance)
                    }

                    if (ModulesInitialized)
                    {
                        var previousView = FindCorrespondingView(RegionNames.MinimumBoundingBoxFinderRegion, previousMinimumBoundingBoxFinderVM.ExportedType);
                        HideView(RegionNames.MinimumBoundingBoxFinderRegion, previousView);
                        ShowView(RegionNames.MinimumBoundingBoxFinderRegion, FindCorrespondingView(RegionNames.MinimumBoundingBoxFinderRegion, minimumBoundingBoxFinderVM.ExportedType));
                    }

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MinimumBoundingBoxFinderVM)));
                }
            }
        }

        /// <summary>
        /// Selected Image Sorter (for a MinimumBoundingBoxFinder)
        /// </summary>
        public ExportedTypeViewModel ImageSorterVM
        {
            get => imageSorterVM;
            set
            {
                var previousImageSorterVM = imageSorterVM;
                imageSorterVM = value;
                if (value != null && ModulesInitialized)
                {
                    var previousView = FindCorrespondingView(RegionNames.ImageSortersRegion, previousImageSorterVM.ExportedType);
                    HideView(RegionNames.ImageSortersRegion, previousView);
                    ShowView(RegionNames.ImageSortersRegion, FindCorrespondingView(RegionNames.ImageSortersRegion, value.ExportedType));
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageSorterVM))); //+ report all the dependent properties
            }
        }

        /// <summary>
        /// Selected Image Sorter (for a Placement Algorithm)
        /// </summary>
        public ExportedTypeViewModel PlacementImageSorterVM
        {
            get => placementImageSorterVM;
            set
            {
                var previousPlacementImageSorterVM = placementImageSorterVM;
                placementImageSorterVM = value;
                if (value != null && ModulesInitialized)
                {
                    var previousView = FindCorrespondingView(RegionNames.PlacementImageSortersRegion, previousPlacementImageSorterVM.ExportedType);
                    HideView(RegionNames.PlacementImageSortersRegion, previousView);
                    ShowView(RegionNames.PlacementImageSortersRegion, FindCorrespondingView(RegionNames.PlacementImageSortersRegion, value.ExportedType));
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlacementImageSorterVM))); //+ report all the dependent properties
            }
        }

        /// <summary>
        /// Selected ColorType
        /// </summary>
        public SKColorType SelectedColorType
        {
            get => selectedColorType;
            set
            {
                if (value == SKColorType.Unknown || value == SKColorType.Alpha8)
                {
                    ShowNotification($@"The {value.ToString()} is color type supported by the underlying graphic librabry but it does not generate any meaningful output.
Please consider using a color type different from {SKColorType.Unknown.ToString()} and {SKColorType.Alpha8.ToString()}.", "Notification");
                }
                selectedColorType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedColorType)));
            }
        }

        /// <summary>
        /// Indicates whether the Selected Minimum Bounding box finder takes a placement algorithm (depends on / could be parametrized by)
        /// </summary>
        public bool SelectedPackerTakesPlacementAlgo
        {
            get => selectedPackerTakesPlacementAlgo;
            set
            {
                selectedPackerTakesPlacementAlgo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPackerTakesPlacementAlgo)));
            }
        }

        /// <summary>
        /// Indicates whether the Selected Minimum Bounding box finder takes an Image Sorter (depends on / could be parametrized by)
        /// </summary>
        public bool SelectedPackerTakesSorter
        {
            get => selectedPackerTakesSorter;
            set
            {
                selectedPackerTakesSorter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPackerTakesSorter)));
                if (selectedPackerTakesSorter && ImageSorterVM == null)
                {
                    ImageSorterVM = ImageSorterVMs?.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Indicates whether the Selected Placement Algorithm takes an Image Sorter (depends on / could be parametrized by)
        /// </summary>
        public bool SelectedPlacementTakesSorter
        {
            get => selectedPlacementTakesSorter;
            set
            {
                selectedPlacementTakesSorter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPlacementTakesSorter)));
                if (selectedPlacementTakesSorter && PlacementImageSorterVM == null)
                {
                    PlacementImageSorterVM = ImageSorterVMs?.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Index (within the Tab Control) of the selected Packing Mode (Basic / Advanced)
        /// </summary>
        public int SelectedPackingSettingsMode
        {
            get => selectedPackingSettingsMode;
            set
            {
                selectedPackingSettingsMode = value;

                ImageSorterVM ??= ImageSorterVMs?.FirstOrDefault();
                PlacementImageSorterVM ??= ImageSorterVM;
                PlacementAlgorithmVM ??= PlacementAlgorithmVMs?.FirstOrDefault();

                //Mode has changed, so we need to adjust the loaded view a little bit
                //if (value == 0) //we are now in basic mode. In ORDER TO AVOID THIS MAGIC CONSTAT, I HAD TO USE static class with constants .. (tried properties in both recx, and xaml..)
                if (value == Common.UIConstants.BASIC_MODE_INDEX)
                {
                    //Just swapped from advanced to basic, so we didnt change BoundaryMode settings in basic settings, but we need to adjust BoundaryBoxFinder accordingly so call command and pass previous boundarymode ..
                    if (SelectBoundaryMode.CanExecute(SelectedBoundaryModeString))
                    {
                        SelectBoundaryMode.Execute(SelectedBoundaryModeString);
                    }
                }
                else if (value == Common.UIConstants.ADVANCED_MODE_INDEX)
                {
                    MinimumBoundingBoxFinderVM ??= MinimumBoundingBoxFinderVMs?.FirstOrDefault(); //for default, we just select the first of the minimum bounding box finders
                }
                else //should never happen
                {
                    throw new InvalidOperationException();
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPackingSettingsMode)));
            }
        }

        /// <summary>
        /// All the types of the placement algorithms that were loaded (imported) from the plugins
        /// </summary>
        public IEnumerable<ExportedTypeViewModel> PlacementAlgorithmVMs
        {
            get => placementAlgorithmVMs?.Where(placementAlgorithmVM =>
            {
                var attr = placementAlgorithmVM.ExportedType.GetCustomAttribute<AvailableToAttribute>();
                return attr == null ||
                       attr.AvailableTo == null ||
                       attr.AvailableTo.Contains(SelectedTargetFramework); //if the attribute is not present at all, it behaves like available to all frameworks, the same for availableto == null ..
            });
            set
            {
                placementAlgorithmVMs = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlacementAlgorithmVMs)));
            }
        }

        /// <summary>
        /// All the types of the minimum bounding box finders that were loaded (imported) from the plugins
        /// </summary>
        public IEnumerable<ExportedTypeViewModel> MinimumBoundingBoxFinderVMs
        {
            get => minimumBoundingBoxFinderVMs?.Where(minimumBoundingBoxFinderVM =>
            {
                var attr = minimumBoundingBoxFinderVM.ExportedType.GetCustomAttribute<AvailableToAttribute>();
                return attr == null ||  //if the attribute is not present at all, it behaves like available to all frameworks, the same for availableto == null ..
                       attr.AvailableTo == null ||
                       attr.AvailableTo.Contains(SelectedTargetFramework);
            });
            set
            {
                minimumBoundingBoxFinderVMs = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MinimumBoundingBoxFinderVMs)));
            }
        }

        /// <summary>
        /// All the instances of the image sorters that were imported from the plugins
        /// </summary>
        public IEnumerable<ExportedTypeViewModel> ImageSorterVMs
        {
            get => imageSorterVMs;
            set
            {
                imageSorterVMs = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageSorterVMs)));
                ImageSorterVM = imageSorterVMs.FirstOrDefault(); //automatically select first, when changed
                PlacementImageSorterVM = ImageSorterVM;
            }
        }

        /// <summary>
        /// All the instances of the image processors that were imported from the plugins
        /// </summary>
        public IEnumerable<IImageProcessor> ImageProcessors
        {
            get; set;
        }

        public IEnumerable<RectangleViewModel> SelectedRectangleVMs
        {
            get; private set;
        }

        /// <summary>
        /// All the types of the metadata writers that were imported from the plugins
        /// </summary>
        public IEnumerable<ExportedTypeViewModel> MetadataWriterVMs
        {
            get => metadataWriterVMs?.Where(metadataWriterVM =>
            {
                var attr = metadataWriterVM.ExportedType.GetCustomAttribute<TargetFrameworkAttribute>();
                return attr == null || attr.TargetFrameworkID == SelectedTargetFramework || attr.TargetsAllFrameworks; //if the attribute is not present at all, it behaves like TargetsAllFrameworks ..
            });
            set
            {
                metadataWriterVMs = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MetadataWriterVMs)));
            }
        }

        /// <summary>
        /// The selected metadata writer
        /// </summary>
        public IMetadataWriter MetadataWriter
        {
            get => metadataWriter;
            set
            {
                metadataWriter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MetadataWriter)));
            }
        }

        /// <summary>
        /// Type of the selected metadata writer
        /// </summary>
        public ExportedTypeViewModel MetadataWriterVM
        {
            get => metadataWriterVM;
            set
            {
                var previousMetadataWriterVM = metadataWriterVM;
                metadataWriterVM = value;

                if (ModulesInitialized)
                {
                    var previousView = FindCorrespondingView(RegionNames.MetadataWritersRegion, previousMetadataWriterVM.ExportedType);
                    HideView(RegionNames.MetadataWritersRegion, previousView);
                    ShowView(RegionNames.MetadataWritersRegion, FindCorrespondingView(RegionNames.MetadataWritersRegion, metadataWriterVM.ExportedType));
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MetadataWriterVM)));
            }
        }

        /// <summary>
        /// All the types of image processors that were imported from plugins
        /// </summary>
        public IEnumerable<ExportedTypeViewModel> ImageProcessorVMs
        {
            get => imageProcessorVMs?.Where(imageProcessorVM =>
            {
                var attr = imageProcessorVM.ExportedType.GetCustomAttribute<AvailableToAttribute>();
                return attr == null || //if the attribute is not present at all, it behaves like available to all frameworks, the same for availableto == null ..
                       attr.AvailableTo == null ||
                       attr.AvailableTo.Contains(SelectedTargetFramework);
            });
            set
            {
                imageProcessorVMs = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageProcessorVMs)));
            }
        }

        /// <summary>
        /// The selected image processor
        /// </summary>
        public IImageProcessor ImageProcessor
        {
            get => imageProcessor;
            set
            {
                imageProcessor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageProcessor)));
            }
        }

        /// <summary>
        /// Type of the selected image processor
        /// </summary>
        public ExportedTypeViewModel ImageProcessorVM
        {
            get => imageProcessorVM;
            set
            {
                var previousImageProcessorVM = imageProcessorVM;
                imageProcessorVM = value;

                if (ModulesInitialized)
                {
                    var previousView = FindCorrespondingView(RegionNames.ImageProcessorsRegion, previousImageProcessorVM.ExportedType);
                    HideView(RegionNames.ImageProcessorsRegion, previousView);
                    ShowView(RegionNames.ImageProcessorsRegion, FindCorrespondingView(RegionNames.ImageProcessorsRegion, imageProcessorVM.ExportedType));
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageProcessorVM)));
            }
        }

        /// <summary>
        /// Fixed with of the texture atlas
        /// </summary>
        /// <remarks>Used when using the fixed size boundary mode</remarks>
        public ushort FixedWidth
        {
            get => fixedWidth;
            set
            {
                fixedWidth = value > (ushort)short.MaxValue ? (ushort)short.MaxValue : value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FixedWidth)));
            }
        }

        /// <summary>
        /// Fixed height of the texture atlas
        /// </summary>
        /// <remarks>Used when using the fixed size boundary mode</remarks>
        public ushort FixedHeight
        {
            get => fixedHeight;
            set
            {
                fixedHeight = value > (ushort)short.MaxValue ? (ushort)short.MaxValue : value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FixedHeight)));
            }
        }

        /// <summary>
        /// String corresponding to a selected boundary mode
        /// </summary>
        public string SelectedBoundaryModeString
        {
            get => selectedBoundaryModeString;
            set
            {
                selectedBoundaryModeString = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedBoundaryModeString)));
            }
        }

        /// <summary>
        /// ID of the selected target framework
        /// </summary>
        public FrameworkID SelectedTargetFramework
        {
            get => selectedTargetFramework;
            set
            {
                selectedTargetFramework = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTargetFramework)));

                //When selected target framework is changed, some functionality may be disabled, we need to create filters ..
                //var attr = MetadataWriterType.GetCustomAttribute<Plugin.Attributes.TargetFramework>();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MetadataWriterVMs)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlacementAlgorithmVMs)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MinimumBoundingBoxFinderVMs)));
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinimumBoundingBoxFinderInstances"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageProcessorVMs)));
                MetadataWriterVM = MetadataWriterVMs?.FirstOrDefault();
                PlacementAlgorithmVM = PlacementAlgorithmVMs?.FirstOrDefault(); //Was not there

                //Select First ..
                if (SelectedBoundaryModeString == Resources.AdvancedModeString)
                {
                    MinimumBoundingBoxFinderVM = MinimumBoundingBoxFinderVMs?.FirstOrDefault(); //Was not there
                }

                //ImageProcessorType = ... NOT Needed (we do not need to select some image processor .. as they are fully optional)

                //if (attr != null && (attr.TargetFrameworkID == selectedTargetFramework || attr.TargetFrameworkID == Plugin.Attributes.FrameworkID.AllFrameworks))
                //{
                //    //currently selected metadata writer targets selected framework, so we do not have to change it ..    
                //}
                //else
                //{
                //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Targ"));
                //}
            }
        }

        /// <summary>
        /// Checks whether any constructor of <paramref name="inspectedType"/> takes exactly arguments of types <paramref name="parameterTypes"/>
        /// </summary>
        /// <param name="inspectedType">Type whose constructors are inspected</param>
        /// <param name="parameterTypes">Types of the arguments</param>
        /// <returns></returns>
        private static bool ConstructorTakesExactlyGivenParameters(Type inspectedType, params Type [] parameterTypes)
        {
            foreach (var ctor in inspectedType.GetConstructors())
            {
                if (ctor.GetParameters().OrderBy(parameter => parameter.ParameterType.FullName)
                                        .Select(parameter => parameter.ParameterType)
                                        .SequenceEqual(parameterTypes.OrderBy(paramType => paramType.FullName)))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks whether any constructor of <paramref name="inspectedType"/> takes (may take also some other) arguments of types <paramref name="parameterTypes"/>
        /// </summary>
        /// <param name="inspectedType">Type whose constructors are inspected</param>
        /// <param name="parameterTypes">Types of the arguments</param>
        /// <returns></returns>
        private static bool ConstructorContainsGivenParameters(Type inspectedType, params Type [] parameterTypes)
        {
            foreach (var ctor in inspectedType.GetConstructors())
            {
                if (!parameterTypes.Except(ctor.GetParameters().Select(param => param.ParameterType)).Any())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// The Unity container
        /// </summary>
        private IUnityContainer IoC
        {
            get;
            set;
        }

        /// <summary>
        /// Tries to resolve a type <paramref name="type"/> as a type <typeparamref name="T"/> from a given unity container <paramref name="container"/>
        /// </summary>
        /// <typeparam name="T">Type resulting from the resolution</typeparam>
        /// <param name="container">Container which is used for resolution</param>
        /// <param name="type">Type to be resolved</param>
        /// <remarks>The method tries to resolve <paramref name="type"/> from the unity container <paramref name="container"/> and casts it to the <typeparamref name="T"/></remarks>
        /// <returns>The resolved instance</returns>
        private T ResolveTypeAs<T>(IUnityContainer container, Type type) where T : class
        {
            if (type == null)
                return null;

            if (!typeof(T).IsAssignableFrom(type))
                throw new InvalidCastException();

            ////then we look if it is PartiallyContained
            //if (Attribute.IsDefined(type, typeof(Plugin.Attributes.PartiallyContained)))
            //{
            //    Plugin.Attributes.PartiallyContained.ParameterOverrides overrides = null;
            //    paramOverrides.TryGetValue(type.FullName, out overrides);
            //    if (overrides != null)
            //    {
            //        //we let container to resolve it, but specify ParameterOverrides ..
            //        container.Resolve(type, type.FullName, overrides.Parameters.Select(paramOverride => new Unity.Resolution.ParameterOverride(paramOverride.name, paramOverride.value)).ToArray()); //for partially overrides, type.FullName is always contract name..
            //    }

            //}
            //else //otherwise we let container to resolve it
            //{
            return (T)container.Resolve(type);
            //}
        }

        /// <summary>
        /// Shows a given view in a region given by the <paramref name="regionName"/>
        /// </summary>
        /// <param name="regionName">The name of the region containing the view</param>
        /// <param name="view">The view</param>
        private void ShowView(string regionName, object view)
        {
            if (view == null) return;
            var collection = regionManager.Regions[regionName];
            collection.Activate(view);
        }

        /// <summary>
        /// Hides a given view in a region given by the <paramref name="regionName"/>
        /// </summary>
        /// <param name="regionName">The name of the region containing the view</param>
        /// <param name="view">The view</param>
        private void HideView(string regionName, object view)
        {
            if (view == null) return;
            var collection = regionManager.Regions[regionName];
            collection.Deactivate(view);
        }
       
        /// <summary>
        /// For a type <paramref name="t"/> it finds a corresponding view in a region given by the <paramref name="regionName"/>
        /// </summary>
        /// <param name="regionName"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private object FindCorrespondingView(string regionName, Type t)
        {
            if (t == null) return null;
            var collection = regionManager.Regions[regionName];
            return collection.GetView(PluginViewWiring.GetViewName(t));
        }

        /// <summary>
        /// Implementation of the IDisposable interface
        /// </summary>
        public void Dispose()
        {
            IoC.Dispose();
        }

        /// <summary>
        /// Event signaling that the main window should be closed
        /// </summary>
        public event Action ShouldClose;

        /// <summary>
        /// Event for the <see cref="INotifyPropertyChanged"/> interface
        /// Notifies the View that some data in this view model was changed
        /// </summary>
        public override event PropertyChangedEventHandler PropertyChanged;

        /// <see cref="Project"/>
        private Project project;

        /// <see cref="TextureAtlasVM"/>
        private TextureAtlasViewModel textureAtlasVM;

        /// <see cref="AllRectsVM"/>
        private AllRectanglesViewModel allRectsVM;

        /// <see cref="LoadedImagesTreeVM"/>
        private LoadedImagesTreeViewModel loadedImagesTreeVM;

        /// <see cref="SaveAtlasPath"/>
        private string saveAtlasPath;

        /// <see cref="SaveMetadataPath"/>
        private string saveMetadataPath;

        /// <see cref="CurrentJob"/>
        private Job currentJob;

        /// <summary>
        /// The cancellation token source used by the tasks
        /// </summary>
        private CancellationTokenSource cancellationTokenSource;

        /// <see cref="SelectedPackingSettingsMode"/>
        private int selectedPackingSettingsMode;

        /// <see cref="FixedWidth"/>
        private ushort fixedWidth;

        /// <see cref="FixedHeight"/>
        private ushort fixedHeight;

        /// <see cref="SelectedPackerTakesPlacementAlgo"/>
        private bool selectedPackerTakesPlacementAlgo;

        /// <see cref="SelectedPackerTakesSorter"/>
        private bool selectedPackerTakesSorter;

        /// <see cref="SelectedPlacementTakesSorter"/>
        private bool selectedPlacementTakesSorter;

        /// <see cref="SelectedBoundaryModeString"/>
        private string selectedBoundaryModeString;

        /// <see cref="IsAliasCreationEnabled"/>
        private bool isAliasCreationEnabled;

        /// <see cref="IsJobCancelling"/>
        private bool isJobCancelling;

        /// <see cref="IsCurrentJobCancellable"/>
        private bool isCurrentJobCancellable;

        /// <see cref="MinimumBoundingBoxFinder"/>
        private IMinimumBoundingBoxFinder minimumBoundingBoxFinder;

        /// <see cref="MetadataWriter"/>
        private IMetadataWriter metadataWriter;

        /// <see cref="ImageProcessor"/>
        private IImageProcessor imageProcessor;


        /// <see cref="SelectedTargetFramework"/>
        private FrameworkID selectedTargetFramework;

        /// <summary>
        /// The region manager used for region manipulation
        /// </summary>
        private readonly IRegionManager regionManager;

        /// <see cref="ImageSorterVMs"/>
        private IEnumerable<ExportedTypeViewModel> imageSorterVMs;

        /// <see cref="MetadataWriterVMs"/>
        private IEnumerable<ExportedTypeViewModel> metadataWriterVMs;

        /// <see cref="MinimumBoundingBoxFinderVMs"/>
        private IEnumerable<ExportedTypeViewModel> minimumBoundingBoxFinderVMs;

        /// <see cref="PlacementAlgorithmVMs"/>
        private IEnumerable<ExportedTypeViewModel> placementAlgorithmVMs;

        /// <see cref="ImageProcessorVMs"/>
        private IEnumerable<ExportedTypeViewModel> imageProcessorVMs;


        /// <see cref="PlacementAlgorithmVM"/>
        private ExportedTypeViewModel placementAlgorithmVM;

        /// <see cref="MinimumBoundingBoxFinderVM"/>
        private ExportedTypeViewModel minimumBoundingBoxFinderVM;

        /// <see cref="MetadataWriterVM"/>
        private ExportedTypeViewModel metadataWriterVM;

        /// <see cref="ImageProcessorVM"/>
        private ExportedTypeViewModel imageProcessorVM;

        /// <see cref="ImageSorterVM"/>
        private ExportedTypeViewModel imageSorterVM;

        /// <see cref="PlacementImageSorterVM"/>
        private ExportedTypeViewModel placementImageSorterVM;

        /// <see cref="SelectedColorType"/>
        private SKColorType selectedColorType;

        /// <summary>
        /// The event aggregator used for communication between view models
        /// </summary>
        private readonly IEventAggregator eventAggregator;

        /// <summary>
        /// The dialog service
        /// </summary>
        private readonly IDialogService dialogService;
    }
}
