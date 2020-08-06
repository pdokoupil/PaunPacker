using System;
namespace PaunPacker.GUI.WPF.Common.Attributes
{
    /// <summary>
    /// Attribute stating that a decorated type is partially contained which means that some of it's constructor dependencies are provided
    /// from a plugin that contains that type while the other should be provided from the PaunPacker.GUI
    /// </summary>
    /// <remarks>This attribute should be used only on Minimum bounding box finders and Placement algorithms</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PartiallyContainedAttribute : Attribute
    {

    }
}
