using System;

namespace PaunPacker.Core
{
    /// <summary>
    /// Interface for extensible components which are reporting progress
    /// </summary>
    public interface IProgressReporter
    {
        /// <summary>
        /// The progress of the component
        /// </summary>
        int Progress { get; }

        /// <summary>
        /// Event which is fired when the progress has changed
        /// </summary>
        event Action<object, int> ProgressChange;

        /// <summary>
        /// Whether the class really reports progress
        /// </summary>
        /// <remarks>It could seem obvious that the class implementing this interface should report progress, but consider a scenario in which there is a FixedSizePacker that reports progress only when the underlying placement algorithm report progrees</remarks>
        bool ReportsProgress { get; }
    }
}
