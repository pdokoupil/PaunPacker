using System;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace PaunPacker.GUI.Dialogs
{
    /// <summary>
    /// The base class for view models of dialogs
    /// </summary>
    class DialogViewModelBase : BindableBase, IDialogAware
    {
        /// <see cref="IconSource"/>
        private string iconSource;

        /// <summary>
        /// Path to a dialog window icon
        /// </summary>
        public string IconSource
        {
            get { return iconSource; }
            set { SetProperty(ref iconSource, value); }
        }

        /// <see cref="Title"/>
        private string title;

        /// <summary>
        /// Title of the dialog window
        /// </summary>
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        /// <summary>
        /// Event representing a request to close the dialog
        /// </summary>
        public event Action<IDialogResult> RequestClose;

        /// <summary>
        /// Raises the <see cref="RequestClose"/> with a dialog result <paramref name="dialogResult"/>
        /// </summary>
        /// <param name="dialogResult">The dialog result</param>
        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        /// <summary>
        /// Determines whether the dialog could be closed
        /// </summary>
        /// <returns>True if the dialog can be closed, false otherwise</returns>
        public virtual bool CanCloseDialog()
        {
            return true;
        }

        /// <summary>
        /// Callback that is called when the dialog is closed
        /// </summary>
        public virtual void OnDialogClosed()
        {

        }

        /// <summary>
        /// Callback that is called when the dialog is opened
        /// </summary>
        /// <param name="parameters">Parameters that were given to the dialog</param>
        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            
        }
    }
}
