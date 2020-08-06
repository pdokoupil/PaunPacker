using System.ComponentModel;
using PaunPacker.Core.ImageProcessing;
using PaunPacker.GUI.WPF.Common;
using Unity;

namespace PaunPacker.Plugins.ViewModels
{
    /// <summary>
    /// The view model for Extruder
    /// </summary>
    class ExtruderViewModel : ViewModelBase
    {
        /// <summary>
        /// Constructs the ExtruderViewModel
        /// </summary>
        /// <remarks>Also performs the registration of Extruder with a specified Amount</remarks>
        /// <param name="unityContainer"></param>
        public ExtruderViewModel(IUnityContainer unityContainer)
        {
            unityContainer.RegisterFactory<Extruder>((_) =>
            {
                return new Extruder(Amount);
            });
        }

        /// <summary>
        /// The amount of pixels for extrude
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
