using System.Collections.Generic;
using System.IO;

namespace PaunPacker.Common
{
    /// <summary>
    /// Class containing utility functions for IO manipulation
    /// </summary>
    static class IOUtilities
    {
        /// <summary>
        /// Returns all the <see cref="FileInfo"/>s (recursively) starting from a root directory specified by <paramref name="rootFolderPath"/>
        /// </summary>
        /// <param name="rootFolderPath">The directory where the recursive search should start</param>
        /// <returns>All the files that were found</returns>
        public static IEnumerable<FileInfo> GetAllFilesRecursively(string rootFolderPath)
        {
            Stack<DirectoryInfo> stack = new Stack<DirectoryInfo>();
            stack.Push(new DirectoryInfo(rootFolderPath));

            while (stack.Count > 0)
            {
                var currDir = stack.Pop();
                foreach (var file in currDir.GetFiles())
                {
                    yield return file;
                }
                foreach (var dir in currDir.GetDirectories())
                {
                    stack.Push(dir);
                }
            }
        }
    }
}
