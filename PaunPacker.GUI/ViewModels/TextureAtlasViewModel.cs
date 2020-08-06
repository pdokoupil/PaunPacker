using CommonServiceLocator;
using PaunPacker.Core.Atlas;
using PaunPacker.Core.Metadata;
using PaunPacker.Core.Types;
using PaunPacker.GUI.Events;
using PaunPacker.GUI.Workarounds;
using PaunPacker.GUI.WPF.Common;
using Prism.Events;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace PaunPacker.GUI.ViewModels
{
    /// <summary>
    /// ViewModel for a texture atlas
    /// </summary>
    sealed class TextureAtlasViewModel : ViewModelBase, IDisposable
    {
        /// <summary>
        /// Constructs the view model for a given <paramref name="textureAtlas"/> with a given <paramref name="colorType"/>
        /// </summary>
        /// <param name="textureAtlas">The texture atlas for which the view model should be created</param>
        /// <param name="colorType">The color type to be used</param>
        public TextureAtlasViewModel(TextureAtlas textureAtlas, SKColorType colorType = SKColorType.Rgba8888)
        {
            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            
            this.textureAtlas = textureAtlas;
            this.colorType = colorType;
            TextureAtlasBitmap = new ImageViewModel(textureAtlas.GetImageWithColorType(this.colorType));
            SelectedRectangles = new HashSet<PPRect>();
            MouseClickPosition = new PPPoint(-1, -1); //make sure that upper left retangle is not selected in the beginning
            PropertyChanged += TextureAtlasViewModel_PropertyChanged;
            //Paint = new RelayCommand((args =>
            //{
            //    using (SKPaint paint = new SKPaint())
            //    {
            //        paint.IsAntialias = true;
            //        var e = (SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs)args;

            //        foreach (var selectedRectangle in SelectedRectangles)
            //        {
            //            e.Surface.Canvas.DrawRect(new SKRectI(selectedRectangle.Left, selectedRectangle.Top, selectedRectangle.Right, selectedRectangle.Bottom), paint);
            //        }
            //        e.Surface.Canvas.Clear(SKColors.Transparent);
            //    }
            //}), (_) => true);
        }

        /// <summary>
        /// The bitmap of the texture atlas
        /// </summary>
        public ImageViewModel TextureAtlasBitmap
        {
            get => textureAtlasBitmapVM;
            private set
            {
                textureAtlasBitmapVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextureAtlasBitmap)));
            }
        }

        /// <summary>
        /// Metadata from the texture atlas
        /// </summary>
        public MetadataCollection MetaData
        {
            get => new MetadataCollection(textureAtlas);
        }

        /// <summary>
        /// Position of the mouse click
        /// </summary>
        public PPPoint MouseClickPosition
        {
            get => mouseClickPosition;
            set
            {
                mouseClickPosition = value;
                SelectedRectangle = FindSelectedRectangle(MouseClickPosition);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MouseClickPosition)));
            }
        }

        /// <summary>
        /// The rectangle that the user has selected (via the TextureAtlasView)
        /// </summary>
        public PPRect? SelectedRectangle
        {
            get => selectedRectangle;
            set
            {
                if (value.HasValue && SelectedRectangles.Contains(value.Value))
                {
                    SelectedRectangles.Remove(value.Value);
                }
                else if (value.HasValue)
                {
                    SelectedRectangles.Add(value.Value);
                    selectedRectangle = value;
                }
                else
                {
                    selectedRectangle = null;
                }

                eventAggregator.GetEvent<RectanglesSelectedEvent>().Publish(new RectanglesSelectedPayload(SelectedRectangles));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRectangles)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRectangle)));
            }
        }

        /// <summary>
        /// All the selected rectangles
        /// </summary>
        public HashSet<PPRect> SelectedRectangles
        {
            get => selectedRectangles;
            set
            {
                selectedRectangles = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRectangles)));
            }
        }

        /// <summary>
        /// Selects all the rectangles in the texture atlas
        /// </summary>
        public void SelectAllRectangles()
        {
            SelectedRectangles.UnionWith(textureAtlas.Rects); // = new HashSet<PPRect>(textureAtlas.Rects);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRectangles)));
        }

        /// <summary>
        /// Unselects all the rectangles in the texture atlas
        /// </summary>
        public void UnselectAllRectangles()
        {
            SelectedRectangles.Clear();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRectangles)));
        }

        /// <inheritdoc />
        public override event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Finds a selected rectangle based on the coordinates of the mouse click
        /// </summary>
        /// <param name="clickPos">The position of the mouse click</param>
        /// <returns>The rectangle at the given position, null if there is no rectangle at the given position</returns>
        private PPRect? FindSelectedRectangle(PPPoint clickPos)
        {
            foreach (var rect in textureAtlas.Rects)
            {
                if (rect.IntersectsWith(clickPos))
                {
                    return rect;
                }
            }
            return null;
        }

        //public ICommand Paint
        //{
        //    get => paintCommand;
        //    set
        //    {
        //        paintCommand = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Paint"));
        //    }
        //}

        /// <summary>
        /// Width of the texture atlas
        /// </summary>
        public int Width
        {
            get => TextureAtlasBitmap.Image.Bitmap.Width;
        }

        /// <summary>
        /// Height of the texture atlas
        /// </summary>
        public int Height
        {
            get => TextureAtlasBitmap.Image.Bitmap.Height;
        }

        /// <summary>
        /// Image representation of the texture atlas
        /// </summary>
        public ImageSource ImageSource
        {
            get => imageSource;
            set
            {
                imageSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageSource)));
            }
        }
           
        /// <summary>
        /// Represents a state whether the borders of the rectangles within the texture atlas are shown or not
        /// </summary>
        public bool AreBordersShown
        {
            get => areBordersShown;
            set
            {
                areBordersShown = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AreBordersShown)));
            }
        }

        /// <summary>
        /// Draws the thin layer above the texture atlas
        /// This layer is used to draw borders around the selected rectangles
        /// And to highlight the selected rectangles
        /// </summary>
        private void Draw()
        {
            if (topSkiaLayerBitmap == null)
            {
                topSkiaLayerBitmap = new SKBitmap(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul);
            }

            topSkiaLayerBitmap.Erase(SKColors.Transparent);
            
            using (SKPaint paint = new SKPaint())
            using (SKCanvas canvas = new SKCanvas(topSkiaLayerBitmap))
            {
                paint.IsAntialias = true;
                DrawSelected(canvas, paint);
                if (AreBordersShown)
                {
                    DrawBorders(canvas, paint);
                }
            }

            ImageSource = topSkiaLayerBitmap.ToWriteableBitmap();
        }

        /// <summary>
        /// Highlights the selected rectangles
        /// </summary>
        /// <param name="canvas">The canvas where the borders should be drawn</param>
        /// <param name="paint">The pain to be used</param>
        private void DrawSelected(SKCanvas canvas, SKPaint paint)
        {
            paint.Style = SKPaintStyle.Fill;
            paint.Color = SKColors.Black.WithAlpha(64);
            foreach (var rect in SelectedRectangles)
            {
                canvas.DrawRect(new SKRectI(rect.Left, rect.Top, rect.Right, rect.Bottom), paint);
            }
        }

        /// <summary>
        /// Draws borders around the selected rectangles
        /// </summary>
        /// <param name="canvas">The canvas where the borders should be drawn</param>
        /// <param name="paint">The pain to be used</param>
        private void DrawBorders(SKCanvas canvas, SKPaint paint)
        {
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 1;

            foreach (var selectedRectangle in textureAtlas.Rects)
            {
                paint.PathEffect = null;
                paint.Color = SKColors.OrangeRed;
                paint.PathEffect = SKPathEffect.CreateDash(new float[] { 10, 10 }, 10);
                canvas.DrawRect(new SKRectI(selectedRectangle.Left, selectedRectangle.Top, selectedRectangle.Right - 1, selectedRectangle.Bottom - 1), paint);
                paint.Color = SKColors.Black;
                paint.PathEffect = SKPathEffect.CreateDash(new float[] { 10, 10 }, 0);
                canvas.DrawRect(new SKRectI(selectedRectangle.Left, selectedRectangle.Top, selectedRectangle.Right - 1, selectedRectangle.Bottom - 1), paint);
            }
        }

        private void TextureAtlasViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedRectangles" || e.PropertyName == "AreBordersShown")
            {
                Draw();
            }
        }

        public void Dispose()
        {
            topSkiaLayerBitmap.Dispose();
        }

        private readonly TextureAtlas textureAtlas;
        private PPPoint mouseClickPosition;
        private PPRect? selectedRectangle;
        private HashSet<PPRect> selectedRectangles;

        private ImageViewModel textureAtlasBitmapVM;
        private SKBitmap topSkiaLayerBitmap;
        private ImageSource imageSource;
        private bool areBordersShown;
        private readonly SKColorType colorType;

        private readonly IEventAggregator eventAggregator;

        //Oops, MVVM violation? Consider drawing into bitmap and then showing it, instead of drawing using PaintSurface ..
        //I decided to use PaintSurface because I thinkg it is going to be faster than create new bitmap every time
        //However, with bitmap, design would be nicer as I would not have to use SKElementPaintBehavior
        //private SKElement skElement;
        //NO, I DECIDED THAT CODE WOULD BE TOO UGLY,, AS I WOULD HAVE TO EXPOSE SKELEMENT SOMEHOW TO VIEWMODEL AND USE INVALIDATEVISUAL
        //AND THAT WOULD END UP BEING INEFFICIENT



        //private ICommand paintCommand;
    }
}
