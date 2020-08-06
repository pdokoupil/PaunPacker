using System;

namespace PaunPacker.GUI.WPF.Common.Attributes
{
    /// <summary>
    /// States for which frameworks the type decorated by this attribute should be available
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AvailableToAttribute : Attribute
    {
        /// <summary>
        /// Creates an attribute stating that a decorated type should be available to frameworks given by <paramref name="availableTo"/>
        /// </summary>
        /// <param name="availableTo">The frameworks</param>
        public AvailableToAttribute(params FrameworkID[] availableTo)
        {
            AvailableTo = availableTo;
        }

        /// <summary>
        /// Creates an attribute stating that a decorated type should be available to all the frameworks
        /// </summary>
        public AvailableToAttribute()
        {
            AvailableTo = null;
        }

        /// <summary>
        /// The frameworks for which the decorated types is available to
        /// </summary>
        public FrameworkID[] AvailableTo { get; private set; }
    }
}
