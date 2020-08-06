using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PaunPacker.GUI.Services
{
    /// <summary>
    /// A service that is used to obtain path of files that user has selected and wants it to open
    /// </summary>
    sealed class OpenFileService : IDisposable
    {
        /// <summary>
        /// Constructs a service with a given file extension filter
        /// </summary>
        /// <param name="filter">Filter for file extensions that the user is able to select</param>
        public OpenFileService(string filter = "")
        {
            this.openFileDialog = new OpenFileDialog
            {
                Filter = filter
            };
        }

        /// <summary>
        /// Returns the paths of files that the user has selected
        /// </summary>
        /// <remarks>Allows to select multiple files</remarks>
        /// <returns>Paths of files that the user wants to open</returns>
        public IEnumerable<string> GetFiles()
        {
            openFileDialog.Multiselect = true;

            List<string> result = null;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                result = openFileDialog.FileNames.ToList();
            }

            return result;
        }

        /// <summary>
        /// Returns the path of file that the user has selected
        /// </summary>
        /// <remarks>Allows to select only a single file</remarks>
        /// <returns>Path of files that the user wants to open</returns>
        public string GetFile()
        {
            openFileDialog.Multiselect = false;
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }

            return null;
        }

        /// <summary>
        /// Implementation of <see cref="IDisposable"/> interface
        /// </summary>
        public void Dispose()
        {
            openFileDialog.Dispose();
        }

        private readonly OpenFileDialog openFileDialog;
    }
}
