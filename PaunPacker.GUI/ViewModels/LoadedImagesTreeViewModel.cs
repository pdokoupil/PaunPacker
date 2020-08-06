using CommonServiceLocator;
using PaunPacker.Core.Types;
using PaunPacker.GUI.Events;
using PaunPacker.GUI.WPF.Common;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PaunPacker.GUI.ViewModels
{
    /// <summary>
    /// ViewModel corresponding to a View which shows all the loaded images in the tree
    /// </summary>
    class LoadedImagesTreeViewModel : ViewModelBase
    {
        /// <summary>
        /// Constructs a View model from a rectangles (images) that were loaded
        /// </summary>
        /// <param name="rects">The loaded rects</param>
        public LoadedImagesTreeViewModel(IEnumerable<PPRect> rects)
        {
            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

            var folderImage = BitmapManager.LoadBitmap("folder.png");
            folderImageVM = folderImage != null ? new ImageViewModel(folderImage) : null;

            //Paths = images.Select(x => x.ImagePath);
            FSEntries = CreateFSEntries(rects);

            Unload = new RelayCommand((x) =>
            {
                //Task.Factory.StartNew(() => 
                //{
                    var node = x as NodeVM;
                    IEnumerable<string> paths = Enumerable.Empty<string>();


                    var nodesToUnload = new Stack<NodeVM>();
                    nodesToUnload.Push(node);

                    while (nodesToUnload.Count > 0)
                    {
                        var currNode = nodesToUnload.Pop();
                        if (currNode.IsDirectory)
                        {
                            foreach (var childNode in currNode.Children)
                            {
                                nodesToUnload.Push(childNode);
                            }
                        }
                        else
                        {
                            paths = paths.Append(currNode.Path);
                        }
                    }

                    eventAggregator.GetEvent<UnloadImagesEvent>().Publish(new UnloadImagesPayload(paths));
                    //only uppon success
                    if (RemoveNode(FSEntries, node))
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FSEntries)));
                    }
                //}, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
            }, (_) => true);

            OpenInFileExplorer = new RelayCommand((x) =>
            {
                var path = ((NodeVM)x).Path;
                var explorerService = new Services.OpenFileExplorer();
                if (System.IO.Directory.Exists(path) || System.IO.File.Exists(path))
                {
                    explorerService.ShowFileExplorer(path);
                }
                else
                {
                    throw new InvalidOperationException($"Error occured, the file system entry {path} does not exist anymore");
                }
                
            }, (_) => true);

            NodeRightClick = new RelayCommand((x) =>
            {
                if (x is NodeVM node)
                {
                    node.IsSelected = true;
                }
            }, (_) => true);
        }

        /// <summary>
        /// File system entries (files / directories)
        /// <remarks>Used to build the tree</remarks>
        /// </summary>
        public ObservableCollection<NodeVM> FSEntries
        {
            get; private set;
        }

        /// <summary>
        /// The command to handle the request to unload selected images
        /// </summary>
        public ICommand Unload { get; private set; }

        /// <summary>
        /// The command to handle the righ-click on item in the treeview
        /// </summary>
        /// <remarks>
        /// Used to select node in tree view on righ-click (before the context menu is shown)
        /// Only for esthetical reasons
        /// </remarks>
        public ICommand NodeRightClick { get; private set; }

        /// <summary>
        /// The command to handle the request to open a selected file/folder in file explorer
        /// </summary>
        public ICommand OpenInFileExplorer { get; private set; }

        /// <inheritdoc />
        public override event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Build the root structure (forrest) with file system entries as nodes
        /// </summary>
        /// <param name="rects">Loaded rectangles (images)</param>
        /// <returns>Collection of roots of the trees</returns>
        private ObservableCollection<NodeVM> CreateFSEntries(IEnumerable<PPRect> rects)
        {
            var separator = System.IO.Path.DirectorySeparatorChar;

            NodeVM currNode = null;

            var rootNodes = new ObservableCollection<NodeVM>();

            foreach (var rect in rects)
            {
                var pathChunks = rect.Image.ImagePath.Split(separator);
                string currentPath = "";

                if (pathChunks.Length == 0)
                    continue;

                currNode = rootNodes.FirstOrDefault(x => x.Name == pathChunks[0]);
                if (currNode == null)
                {
                    currNode = new NodeVM { Name = pathChunks[0], Children = new ObservableCollection<NodeVM>(), Depth = 0, Path = pathChunks[0], IsDirectory = true, Thumbnail = folderImageVM };
                    rootNodes.Add(currNode);
                }

                currentPath = pathChunks[0];

                for (int i = 1; i < pathChunks.Length; i++)
                {
                    currentPath += System.IO.Path.DirectorySeparatorChar + pathChunks[i];

                    //traverse current structure
                    NodeVM nextNode = null;
                    if ((nextNode = currNode?.Children.FirstOrDefault(x => x.Name == pathChunks[i])) == null)
                    {
                        nextNode = new NodeVM { Name = pathChunks[i], Children = new ObservableCollection<NodeVM>(), Depth = i, Path = currentPath, IsDirectory = i < pathChunks.Length - 1 };
                        if (i == pathChunks.Length - 1)
                        {
                            nextNode.Thumbnail = new ImageViewModel(rect.Image);
                        }
                        else
                        {
                            nextNode.Thumbnail = folderImageVM;
                        }
                        currNode.Children.Add(nextNode);
                    }
                    currNode = nextNode;
                }
            }

            var rootsToRemove = new List<NodeVM>();
            //Normalize the paths
            for (int i = 0; i < rootNodes.Count; i++)
            {
                int j = 0;
                var normalizedPath = rootNodes[i].Name;
                NodeVM parentNode = null;
                var currentNode = rootNodes[i];

                while (currentNode.Children.Count == 1)
                {
                    normalizedPath = currentNode.Children[0].Path;
                    parentNode = currentNode;
                    currentNode = currentNode.Children[0];
                    j++;
                }

                //Path was changed
                if (j > 0)
                {
                    //Delete empty directory
                    if (currentNode.Children.Count == 0 && currentNode.IsDirectory)
                    {
                        rootsToRemove.Add(rootNodes[i]);
                    }
                    else
                    {
                        if (parentNode != null)
                        {
                            parentNode.Children.Clear();
                            parentNode = null;
                        }

                        currentNode.Name = currentNode.Path;
                        rootNodes[i] = currentNode;
                        //Update the depths for all the children (recursively) of the currentNode

                        int decreaseAmount = currentNode.Depth;
                        var decreaseDepthStack = new Stack<NodeVM>();
                        decreaseDepthStack.Push(currentNode);

                        while (decreaseDepthStack.Count > 0)
                        {
                            currentNode = decreaseDepthStack.Pop();
                            currentNode.Depth -= decreaseAmount;
                            foreach (var child in currentNode.Children)
                            {
                                decreaseDepthStack.Push(child);
                            }
                        }
                    }
                }
            }

            foreach (var rootToRemove in rootsToRemove)
            {
                rootNodes.Remove(rootToRemove);
            }

            return rootNodes;
        }
       
        /// <summary>
        /// Removes the node from the tree (forrest)
        /// </summary>
        /// <param name="roots">The roots of the tree</param>
        /// <param name="toRemove">The node to remove</param>
        /// <returns>True if the node was found and removed, false otherwise</returns>
        private bool RemoveNode(ObservableCollection<NodeVM> roots, NodeVM toRemove)
        {
            if (roots == null)
            {
                return false;
            }

            foreach (var root in roots)
            {
                if (root.Path == toRemove.Path)
                {
                    roots.Remove(root);
                    return true;
                }
                else if (RemoveNode(root.Children, toRemove))
                {
                    if (root.Children.Count == 0)
                    {
                        roots.Remove(root); //propagate up ..
                    }
                    return true;
                }
            }

            return false;
        }

        private readonly ImageViewModel folderImageVM;
        private readonly IEventAggregator eventAggregator;
    }

    /// <summary>
    /// The ViewModel of a single node within the FSEntries tree
    /// </summary>
    class NodeVM : ViewModelBase
    {
        /// <summary>
        /// Name of the FSEntry
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Thumbnail of the FSEntry
        /// </summary>
        public ImageViewModel Thumbnail { get; set; }

        /// <summary>
        /// Path of the FSEntry
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Depth (within the tree) of the FSEntry
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Determines whether the node is directory or not (file)
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// Children of the FSEntry
        /// </summary>
        public ObservableCollection<NodeVM> Children { get; set; }

        /// <summary>
        /// Removes the <paramref name="children"/>
        /// </summary>
        /// <param name="children">The children to be removed</param>
        public void RemoveChildren(NodeVM children)
        {
            Children.Remove(children);
        }

        /// <summary>
        /// Determines whether this node is selected in the TreeView
        /// </summary>
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
            }
        }

        /// <inheritdoc />
        public override event PropertyChangedEventHandler PropertyChanged;

        /// <see cref="IsSelected"/>
        private bool isSelected;
    }
}
