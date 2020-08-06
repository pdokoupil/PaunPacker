using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PaunPacker
{
    /// <summary>
    /// Performs manipulation with projects
    /// </summary>
    static class ProjectManager
    {
        /// <summary>
        /// Creates an empty project with a given project name and at a given path
        /// </summary>
        /// <param name="projectName">The name of the project</param>
        /// <param name="path">The path of the project</param>
        /// <returns>The created project</returns>
        public static Project CreateProject(string projectName, string path)
        {
            Project p = new Project { Name = projectName, Path = path };
            SaveProject(p);
            return p;
        }

        /// <summary>
        /// Saves the given project at a given path
        /// </summary>
        /// <param name="p">Project to be saved</param>
        /// <param name="newPath">Path where the project should be saved</param>
        public static void SaveProject(Project p, string newPath = null)
        {
            string finalPath = newPath ?? p.Path;
            //do save
            writer.Write(p, finalPath);
        }

        /// <summary>
        /// Loads a project from a project file at a given path
        /// </summary>
        /// <param name="path">Path of the project file</param>
        /// <param name="errorMessage">Contains the error message describing the error which occured during the project file load. If there was no error, the <paramref name="errorMessage"/> is null</param>
        /// <remarks>Either the returned value is null and <paramref name="errorMessage"/> is not null or vice versa</remarks>
        /// <returns>The loaded project, null if the project file is corrupted</returns>
        public static Project LoadProject(string path, out string errorMessage)
        {
            return loader.Load(path, out errorMessage);
        }

        /// <summary>
        /// Loads the projects
        /// </summary>
        class ProjectLoader
        {
            /// <summary>
            /// Constructs a ProjectLoader
            /// </summary>
            public ProjectLoader()
            {
                serializer = new XmlSerializer(typeof(Project));
                serializer.UnknownAttribute += Serializer_UnknownAttribute;
                serializer.UnknownElement += Serializer_UnknownElement;
                serializer.UnknownNode += Serializer_UnknownNode;
            }

            /// <summary>
            /// Handles the UnknownNode event
            /// </summary>
            private void Serializer_UnknownNode(object sender, XmlNodeEventArgs e)
            {
                throw new InvalidOperationException($"Unknown node: {e.Name} at line: {e.LineNumber} and position: {e.LinePosition}");
            }

            /// <summary>
            /// Handles the UnknownAttribute event
            /// </summary>
            private void Serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
            {
                throw new InvalidOperationException($"Unknown attribute: {e.Attr.Name} at line: {e.LineNumber} and position: {e.LinePosition}");
            }

            /// <summary>
            /// Handles the UnknownElement event
            /// </summary>
            private void Serializer_UnknownElement(object sender, XmlElementEventArgs e)
            {
                throw new InvalidOperationException($"Unknown element: {e.Element.Name} at line: {e.LineNumber} and position: {e.LinePosition}");
            }

            /// <summary>
            /// Loads a project from a project file at a given path
            /// </summary>
            /// <param name="path">The path of the project file</param>
            /// <param name="errorMessage">Contains the error message describing the error which occured during the project file load. If there was no error, the <paramref name="errorMessage"/> is null</param>
            /// <remarks>Either the returned value is null and <paramref name="errorMessage"/> is not null or vice versa</remarks>
            /// <returns>The loaded project, null if the project file is corrupted</returns>
            public Project Load(string path, out string errorMessage)
            {
                using var sr = new StreamReader(path);
                using var xmlReader = XmlReader.Create(sr);

                Project p = null;
                errorMessage = null;

                try
                {
                    p = (Project)serializer.Deserialize(xmlReader);
                }
                catch (InvalidOperationException e)
                {
                    errorMessage = e.Message;
                }

                return p;
            }
            private readonly XmlSerializer serializer;
        }

        /// <summary>
        /// Saves the projects
        /// </summary>
        class ProjectWriter
        {
            /// <summary>
            /// Constructs the ProjectWriter
            /// </summary>
            public ProjectWriter()
            {
                serializer = new XmlSerializer(typeof(Project));
            }

            /// <summary>
            /// Saves the project <paramref name="p"/> at a given path
            /// </summary>
            /// <param name="p">The project to be saved</param>
            /// <param name="path">The path where the project should be saved</param>
            public void Write(Project p, string path)
            {
                using var sr = new StreamWriter(path);
                serializer.Serialize(sr, p);
            }
            private readonly XmlSerializer serializer;
        }

        private static readonly ProjectLoader loader = new ProjectLoader();
        private static readonly ProjectWriter writer = new ProjectWriter();
    }
}
