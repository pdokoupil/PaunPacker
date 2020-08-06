using PaunPacker.Core.Types;
using PaunPacker.GUI.WPF.Common;
using System.Windows.Controls;
using PaunPacker.GUI.Workarounds;

namespace PaunPacker.GUI.ViewModels
{
    /// <summary>
    /// ViewModel corresponding to a single rectangle
    /// </summary>
    class RectangleViewModel : ViewModelBase
    {
        /// <summary>
        /// Creates a ViewModel for a given <paramref name="rect"/>
        /// </summary>
        /// <param name="rect">The rectangle for which the view model should be created</param>
        public RectangleViewModel(PPRect rect)
        {
            Rectangle = rect;
        }

        /// <summary>
        /// Image representation of the rectangle
        /// </summary>
        public Image ImageRepresentation
        {
            get
            {
                if (Rectangle.Image != null)
                {
                    var resultImage = new Image();
                    var bmp = Rectangle.Image.Bitmap.ToWriteableBitmap();
                    resultImage.Source = bmp; return resultImage;
                }
                else
                {
                    var resultImage = new Image();
                    return resultImage; //todo, create single solid color image (with random color? for rectangles without specified image)
                }
                
            }
        }

        /// <summary>
        /// The rectangle represented by this view model
        /// <remarks>This is direct exposure of the model (in terms of MVVM)</remarks>
        /// </summary>
        public PPRect Rectangle { get; private set; }
    }
}
