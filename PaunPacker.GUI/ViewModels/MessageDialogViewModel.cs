using PaunPacker.GUI.Dialogs;
using Prism.Services.Dialogs;

namespace PaunPacker.GUI.ViewModels
{
    /// <summary>
    /// The view model for the dialog that shows a message
    /// </summary>
    /// <remarks>Uses prism's View Model locator to get instantiated</remarks>
#pragma warning disable CA1812 //Instantiated by the Prism's ViewModelLocator
    class MessageDialogViewModel : DialogViewModelBase
    {
#pragma warning enable CA1812
        /// <summary>
        /// Constructs the view model
        /// </summary>
        public MessageDialogViewModel()
        {
            Title = "Message";
            CloseDialogCommand = new RelayCommand((_) => CloseDialog(), (_) => true);
        }

        /// <see cref="Message"/>
        private string message;

        /// <summary>
        /// The message shown in the dialog
        /// </summary>
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        /// <summary>
        /// Command that is executed when the dialog is closed
        /// </summary>
        public RelayCommand CloseDialogCommand { get; set; }

        /// <summary>
        /// Requests a close of the dialog with a "true" DialogResult
        /// </summary>
        private void CloseDialog()
        {
            RaiseRequestClose(new DialogResult(true));
        }

        /// <inheritdoc />
        public override void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>(MessageDialogParameterNames.Message);
            var title = parameters.GetValue<string>(MessageDialogParameterNames.Title);
            if (title != null)
            {
                Title = title;
            }
        }
    }
}
