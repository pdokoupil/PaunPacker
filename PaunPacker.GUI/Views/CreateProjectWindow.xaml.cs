using System.Windows;
using PaunPacker.Common;
using PaunPacker.GUI.Dialogs;
using PaunPacker.GUI.Services;
using Prism.Services.Dialogs;

namespace PaunPacker.GUI.Views
{
    /// <summary>
    /// Interaction logic for CreateProjectWindow.xaml
    /// </summary>
    public partial class CreateProjectWindow : Window
    {
        public CreateProjectWindow(IDialogService dialogService)
        {
            InitializeComponent();
            this.dialogService = dialogService;
        }

        private void ShowError(string message)
        {
            var parameters = new DialogParameters()
            {
                { MessageDialogParameterNames.Title, "Error"},
                { MessageDialogParameterNames.Message, message }
            };
            dialogService.ShowDialog(DialogNames.MessageDialog, parameters, (x) => { });
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            //var res = Renderer.RenderSaveDialog();
            //if (res != null)
            //    projectPathTextBox.Text = res;
            using var saveFileService = new SaveFileService(FileFilters.ProjectExtensionFilter);
            projectPathTextBox.Text = saveFileService.GetFile();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (projectPathTextBox.Text == null || projectPathTextBox.Text.Length == 0  || !System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(projectPathTextBox.Text)))
            {
                ShowError("You must select a valid project path !");
                return;
            }
            if (projectNameTextBox.Text == null || projectNameTextBox.Text.Length == 0) //check if default i.e. empty text box is null or "" and based on the observation, eliminate one of the conditions
            {
                ShowError("You must enter a project name !");
                return;
            }
            //ProjectManager pm = new ProjectManager();
            DialogResult = true;
            CreatedProject = ProjectManager.CreateProject(projectNameTextBox.Text,projectPathTextBox.Text);
            Close();
        }

        internal Project CreatedProject { get; set; }
        private readonly IDialogService dialogService;
    }
}
