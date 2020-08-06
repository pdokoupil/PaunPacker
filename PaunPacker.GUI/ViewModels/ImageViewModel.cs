using PaunPacker.Core.Types;
using PaunPacker.GUI.WPF.Common;

namespace PaunPacker.GUI.ViewModels
{
    /// <summary>
    /// ViewModel corresponding to a view for a single PPImage
    /// </summary>
    class ImageViewModel : ViewModelBase
    {
        /// <summary>
        /// Constructs the ViewModel from a <paramref name="image"/>
        /// </summary>
        /// <param name="image">Image for which the ViewModel is constructed</param>
        public ImageViewModel(PPImage image)
        {
            Image = image;
        }

        /// <summary>
        /// The image represented by this ViewModel
        /// </summary>
        public PPImage Image { get; private set; }
    }
}
