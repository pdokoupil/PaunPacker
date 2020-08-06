using Prism.Events;
using System.Collections.Generic;

namespace PaunPacker.GUI.Events
{
    /// <summary>
    /// Class representing an event that certain images should be unloaded from the application
    /// </summary>
    /// <remarks>This event is raised by the LoadedImagesTreeViewModel and processed by the AllRectanglesViewModel</remarks>
    public class UnloadImagesEvent : PubSubEvent<UnloadImagesPayload>
    {

    }

    /// <summary>
    /// This class represents a payload for the <see cref="UnloadImagesEvent"/>
    /// </summary>
    public class UnloadImagesPayload
    {
        /// <summary>
        /// Constructs new payload from a paths of images that should be unloaded
        /// </summary>
        /// <param name="imagePaths">Paths of images that should be unloaded</param>
        public UnloadImagesPayload(IEnumerable<string> imagePaths)
        {
            ImagePaths = imagePaths;
        }

        /// <summary>
        /// Paths of images that should be unloaded
        /// </summary>
        public IEnumerable<string> ImagePaths { get; private set; }
    }
}
