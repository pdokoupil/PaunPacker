using System.ComponentModel;
using PaunPacker.Core.ImageProcessing.ImageProcessors;
using PaunPacker.GUI.WPF.Common;
using Unity;

namespace PaunPacker.Plugins.ViewModels
{
    /// <summary>
    /// The view model for a cropping trimmer
    /// </summary>
    class CroppingTrimmerViewModel : ViewModelBase
    {
        /// <summary>
        /// Constructs the TrimmerViewModel
        /// </summary>
        /// <remarks>Also performs the registration of Trimmer with a specified Alpha tolerance</remarks>
        /// <param name="unityContainer"></param>
        public CroppingTrimmerViewModel(IUnityContainer unityContainer)
        {
            unityContainer.RegisterFactory<CroppingTrimmer>((_) =>
            {
                return new CroppingTrimmer(AlphaTolerance);
            });
        }

        /// <summary>
        /// The Alpha tolerance
        /// Pixels with alpha less than or equal to this treshold will be treated as transparent
        /// </summary>
        /// <remarks>Alpha tolerance is a number between 0 and 255. Values greater than 255 are treated as 255</remarks>
        public byte AlphaTolerance
        {
            get => alphaTolerance;
            set
            {
                alphaTolerance = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlphaTolerance)));
            }
        }

        /// <summary>
        /// The PropertyChangedEvent used to notify the view about data changes
        /// </summary>
        public override event PropertyChangedEventHandler PropertyChanged;

        /// <see cref="AlphaTolerance"/>
        private byte alphaTolerance;
    }
}
