using System.ComponentModel;
using PaunPacker.Core.ImageProcessing.ImageProcessors;
using PaunPacker.GUI.WPF.Common;
using Unity;

namespace PaunPacker.Plugins.ViewModels
{
    /// <summary>
    /// The view model for PaddingAdder
    /// </summary>
    class PaddingAdderViewModel : ViewModelBase
    {
        /// <summary>
        /// Constructs the PaddingAdderViewModel
        /// </summary>
        /// <remarks>Also performs the registration of PaddingAdder with a specified Amount</remarks>
        /// <param name="unityContainer"></param>
        public PaddingAdderViewModel(IUnityContainer unityContainer)
        {
            unityContainer.RegisterFactory<PaddingAdder>((_) =>
            {
                return new PaddingAdder(Amount);
            });
        }

        /// <summary>
        /// The amount of pixels for padding
        /// </summary>
        public byte Amount
        {
            get => amount;
            set
            {
                amount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Amount)));
            }
        }

        /// <summary>
        /// The PropertyChangedEvent used to notify the view about data changes
        /// </summary>
        public override event PropertyChangedEventHandler PropertyChanged;

        /// <see cref="Amount"/>
        private byte amount;
    }
}
