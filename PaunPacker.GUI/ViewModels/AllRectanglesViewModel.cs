using CommonServiceLocator;
using MoreLinq;
using PaunPacker.Core.Types;
using PaunPacker.GUI.Events;
using PaunPacker.GUI.WPF.Common;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace PaunPacker.GUI.ViewModels
{
    /// <summary>
    /// ViewModel managing all the loaded rectangles (images)
    /// </summary>
    class AllRectanglesViewModel : ViewModelBase
    {
        /// <summary>
        /// Constructs the view model from the loaded rectangles
        /// </summary>
        /// <param name="rects">The rectangles</param>
        public AllRectanglesViewModel(IEnumerable<RectangleViewModel> rects)
        {
            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAggregator.GetEvent<UnloadImagesEvent>().Subscribe(imagePathsPayload =>
            {
                foreach (var imgPath in imagePathsPayload.ImagePaths)
                {
                    var found = AllRectangles.Where(rect => rect.Rectangle.Image != null && rect.Rectangle.Image.ImagePath == imgPath).FirstOrDefault();
                    if (found != null)
                    {
                        AllRectangles.Remove(found);
                    }
                }

                Rectangles = AllRectangles.Select(x => x.Rectangle);
            });

            AllRectangles = new ObservableCollection<RectangleViewModel>(rects);
            Rectangles = rects.Select(x => x.Rectangle);
        }

        /// <summary>
        /// ViewModels of the loaded rectangles
        /// </summary>
        public ObservableCollection<RectangleViewModel> AllRectangles
        {
            get; private set;
        }

        /// <summary>
        /// The loaded rectangles
        /// </summary>
        public IEnumerable<PPRect> Rectangles
        {
            get => rectangles;
            private set
            {
                rectangles = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Rectangles)));
            }
        }

        /// <summary>
        /// Checks for occurences of rectangles from <paramref name="rectanglesToUpdate"/> within <see cref="AllRectangles"/> and for each occurence
        /// The occurence in <see cref="AllRectangles"/> is replaced by the occurence in <paramref name="rectanglesToUpdate"/>
        /// </summary>
        /// <param name="rectanglesToUpdate"></param>
        public void UpdateRectangles(IEnumerable<RectangleViewModel> rectanglesToUpdate)
        {
            bool modified = false;

            foreach (var rectangle in rectanglesToUpdate)
            {
                var indexOfRectangle = AllRectangles.Select((rect, index) => (rect, index))
                    .FirstOrDefault(pair => pair.rect.Rectangle.Image?.ImagePath == rectangle.Rectangle.Image?.ImagePath).index;
                
                if (indexOfRectangle == 0)
                {
                    if (!(AllRectangles.Count > 0 &&
                        AllRectangles[0].Rectangle.Image?.ImagePath == rectangle.Rectangle.Image?.ImagePath))
                    {
                        //Rectangle not found
                        continue;
                    }
                }

                if (rectangle.Rectangle.Width == 0 || rectangle.Rectangle.Height == 0)
                {
                    AllRectangles.RemoveAt(indexOfRectangle);
                    modified = true;
                    continue;
                }

                AllRectangles[indexOfRectangle] = rectangle;
                modified = true;
            }

            if (modified)
            {
                Rectangles = AllRectangles.Select(x => x.Rectangle);
            }
        }

        /// <summary>
        /// Contains the same rectangles as the <see cref="Rectangles"/> but grupped by the hash of the image that they correspond to
        /// </summary>
        public IEnumerable<IGrouping<string, RectangleViewModel>> RectanglesByHash => AllRectangles.GroupBy(x => x.Rectangle.Image.BitmapHash);


        /// <inheritdoc />
        public override event PropertyChangedEventHandler PropertyChanged;

        /// <see cref="Rectangles"/>
        private IEnumerable<PPRect> rectangles;

        /// <summary>
        /// The event aggregator
        /// </summary>
        private readonly IEventAggregator eventAggregator;
    }
}
