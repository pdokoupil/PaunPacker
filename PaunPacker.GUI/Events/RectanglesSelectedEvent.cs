using System;
using System.Collections.Generic;
using System.Text;
using PaunPacker.Core.Types;
using Prism.Events;

namespace PaunPacker.GUI.Events
{
    /// <summary>
    /// This class represents an event that a selection of rectangles (images) within the texture atlas has changed
    /// </summary>
    /// <remarks>This event is raised by the TextureAtlasViewModel and processed by the MainWindowViewModel</remarks>
    public class RectanglesSelectedEvent : PubSubEvent<RectanglesSelectedPayload>
    {

    }

    /// <summary>
    /// The payload for the <see cref="RectanglesSelectedEvent"/>
    /// </summary>
    public class RectanglesSelectedPayload
    {
        /// <summary>
        /// Constructs new payload with rectangles (images) that are currently selected
        /// </summary>
        /// <param name="selectedRectangles">The rectangles currently being selected</param>
        public RectanglesSelectedPayload(IEnumerable<PPRect> selectedRectangles)
        {
            SelectedRectangles = selectedRectangles;
        }

        /// <summary>
        /// The rectangles that are currently selected
        /// </summary>
        public IEnumerable<PPRect> SelectedRectangles { get; private set; }
    }
}
