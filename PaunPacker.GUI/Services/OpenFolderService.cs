using System;
using System.Windows.Forms;

namespace PaunPacker.GUI.Services
{
    /// <summary>
    /// A service that is used to obtain path of a directory that user has selected and wants it to open
    /// </summary>
    sealed class OpenFolderService : IDisposable
    {
        public OpenFolderService()
        {
            folderBrowserDialog = new FolderBrowserDialog();
        }

        /// <summary>
        /// Returns the path of directory that the user has selected
        /// </summary>
        /// <remarks>Allows to select only a single directory</remarks>
        /// <returns>Paths of directory that the user wants to open</returns>
        public string GetFolder()
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                return folderBrowserDialog.SelectedPath;
            }
            return null;
        }

        /// <summary>
        /// Implementation of <see cref="IDisposable"/> interface
        /// </summary>
        public void Dispose()
        {
            folderBrowserDialog.Dispose();
        }

        private readonly FolderBrowserDialog folderBrowserDialog;
    }
}
