using System;

namespace PaunPacker.GUI.WPF.Common.Attributes
{
    /// <summary>
    /// States that a decorated metadata writer exports a metadata into a format of a given framework
    /// </summary>
    /// <remarks>
    /// Should only decorate the metadata writers
    /// This attribute leverages which features should be made available to the user
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TargetFrameworkAttribute : Attribute
    {
        /// <summary>
        /// Creates the attribute stating that a decorated metadata framework targets a framework <paramref name="targetFramework"/> and that the name (displayed in GUI) of this framework is <paramref name="frameworkName"/>
        /// </summary>
        /// <param name="targetFramework">The ID of the target framework</param>
        /// <param name="frameworkName">Specifies the name of the target framework (typically used in conjunction with FrameworkID.Custom)</param>
        public TargetFrameworkAttribute(FrameworkID targetFramework, string frameworkName = null)
        {
            TargetFrameworkID = targetFramework;
            FrameworkName = frameworkName;
        }

        /// <summary>
        /// Creates a metadata writer targetting all the frameworks
        /// </summary>
        /// <remarks>This is useful when creating a metadata writer that exports to some custom format and that is capable of exporting anything (therefore allowing the user to use arbitrary feature)</remarks>
        public TargetFrameworkAttribute()
        {
            TargetsAllFrameworks = true;
        }

        /// <summary>
        /// Indicates whether the decorated metadata writer targets all the frameworks
        /// </summary>
        public bool TargetsAllFrameworks { get; private set; }

        /// <summary>
        /// ID of the target framework
        /// </summary>
        public FrameworkID TargetFrameworkID { get; private set; }

        /// <summary>
        /// Name of the target framework
        /// </summary>
        public string FrameworkName { get; private set; }
    }
}
