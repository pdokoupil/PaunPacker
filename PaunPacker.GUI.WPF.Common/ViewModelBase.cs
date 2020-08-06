using System.ComponentModel;

namespace PaunPacker.GUI.WPF.Common
{
    /// <summary>
    /// The common base class for all the view models
    /// Inherits the <see cref="INotifyPropertyChanged"/> interface
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// This event should be used to notify the view corresponding the the view model implementing this interface about changes in view model's data
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged;
    }
}
