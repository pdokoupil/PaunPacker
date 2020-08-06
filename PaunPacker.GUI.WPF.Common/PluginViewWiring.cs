using System;

namespace PaunPacker.GUI.WPF.Common
{
    /// <summary>
    /// This class provides a mechanism to wire the type (exported from a plugin) to it's view
    /// By exposing a function that from a <see cref="Type"/> of an exported type, could return a name of the view
    /// under which the view should be registered into IoC
    /// </summary>
    public static class PluginViewWiring
    {
        /// <summary>
        /// For a given <paramref name="type"/>, returns the name of it's corresponding view that should be used to register/resolve the view from IoC
        /// </summary>
        /// <param name="type">The type of the exported type</param>
        /// <exception cref="ArgumentNullException">Is thrown when the <paramref name="type"/> is null</exception>
        /// <returns>The name of the view</returns>
        public static string GetViewName(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException($"The {nameof(type)} cannot be null");
            }
            return type.FullName + "View";
        }
    }
}
