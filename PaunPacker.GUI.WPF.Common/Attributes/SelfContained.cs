using System;

namespace PaunPacker.GUI.WPF.Common.Attributes
{
    /// <summary>
    /// Attribute stating that a decorated type is self (fully) contained which means that all of it's constructor dependencies are provided
    /// from a plugin that contains that type
    /// </summary>
    /// <remarks>
    /// This attribute should be used only on Minimum bounding box finders and Placement algorithms
    /// If type has this attribute on it, it is telling that user should not be able to inject Placement/Sorter into it
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SelfContainedAttribute : Attribute
    {
        
    }
}
