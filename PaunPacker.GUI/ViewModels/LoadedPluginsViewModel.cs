using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using PaunPacker.GUI.Dialogs;
using Prism.Services.Dialogs;

namespace PaunPacker.GUI.ViewModels
{
    /// <summary>
    /// The view model for the dialog that shows loaded plugins
    /// </summary>
    /// <remarks>Uses prism's View Model locator to get instantiated</remarks>
#pragma warning disable CA1812 //Instantiated by the Prism's ViewModelLocator
    class LoadedPluginsViewModel : DialogViewModelBase
    {
#pragma warning enable CA1812
        /// <summary>
        /// Constructs a view model passing in the dialog service
        /// </summary>
        /// <param name="dialogService">The dialog service</param>
        public LoadedPluginsViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            CloseDialogCommand = new RelayCommand((_) => CloseDialog(), (_) => true);
            LoadedPlugins = new ObservableCollection<PluginViewModel>();

            ShowDetails = new RelayCommand((x) =>
            {
                var parameters = new DialogParameters()
                {
                    { "pluginViewModel", SelectedPlugin}
                };
                this.dialogService.ShowDialog(DialogNames.PluginDetailsDialog, parameters, (_) => { });
            }, (_) => true);
        }

        /// <inheritdoc />
        public override void OnDialogOpened(IDialogParameters parameters)
        {
            var title = parameters.GetValue<string>(LoadedPluginsDialogParameterNames.Title);
            if (title != null)
            {
                Title = title;
            }
            var loadedPlugins = parameters.GetValue<IEnumerable<PluginViewModel>>(LoadedPluginsDialogParameterNames.LoadedPlugins);
            if (loadedPlugins != null)
            {
                LoadedPlugins.Clear();
                LoadedPlugins.AddRange(loadedPlugins);
            }
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

        /// <summary>
        /// Command handling the click event on "ShowDetails" button
        /// </summary>
        public ICommand ShowDetails { get; private set; }

        /// <summary>
        /// The plugin that is selected by the user (in the listview)
        /// </summary>
        public PluginViewModel SelectedPlugin { get; set; }

        /// <summary>
        /// View models of all the loaded plugins
        /// </summary>
        public ObservableCollection<PluginViewModel> LoadedPlugins { get; private set; }

        /// <summary>
        /// The dialog service
        /// </summary>
        private readonly IDialogService dialogService;
    }
}
