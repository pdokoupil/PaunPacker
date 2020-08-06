using System;
using System.Windows.Forms;

namespace PaunPacker.GUI.Services
{
    /// <summary>
    /// A service that is used to obtain path to save the file at location specified by the user
    /// </summary>
    sealed class SaveFileService : IDisposable
    {
        /// <summary>
        /// Constructs a service with a given file extension filter
        /// </summary>
        /// <param name="filter">The file extension filter</param>
        public SaveFileService(string filter = "")
        {
            this.saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = filter;
        }

        /// <summary>
        /// Returns the path where the file should be saved
        /// </summary>
        /// <returns></returns>
        public string GetFile()
        {

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }
            return null;
        }

        /// <summary>
        /// Implementation of <see cref="IDisposable"/> interface
        /// </summary>
        public void Dispose()
        {
            saveFileDialog.Dispose();
        }

        private readonly SaveFileDialog saveFileDialog;
    }
}
