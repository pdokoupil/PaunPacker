
namespace PaunPacker.GUI.WPF.Common.Attributes
{
    /// <summary>
    /// Identification of a framework
    /// </summary>
    /// <remarks>A frameworks that are not listed by this enum are meant to be <see cref="Custom"/>
    /// So when plugin developers are developing for a framework not listed in this enum they should use AvailableTo(FrameworkID.Custom) which is (at the moment) equivalent to AvailableTo()</remarks>
    public enum FrameworkID {
        /// <summary>
        /// Corresponds to the Unity game engine
        /// </summary>
        Unity,
        /// <summary>
        /// Corresponds to libGDX
        /// </summary>
        libGDX,
        /// <summary>
        /// Corresponds to anything else
        /// </summary>
        Custom
    };
}
