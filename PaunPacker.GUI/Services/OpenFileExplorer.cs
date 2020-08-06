using System.Diagnostics;

namespace PaunPacker.GUI.Services
{
    /// <summary>
    /// Service used to open file explorer and select a given path (if the path is directory, the file
    /// explorer lists the contents of this folder otherwise, if it is a file, the file is selected)
    /// </summary>
    class OpenFileExplorer
    {
        /// <summary>
        /// Shows the file explorer and selects the <paramref name="path"/>
        /// </summary>
        /// <param name="path">A path of file / directory that should be selected</param>
        public void ShowFileExplorer(string path)
        {
            Process.Start("explorer.exe", "/select," + path);
        }
    }
}
