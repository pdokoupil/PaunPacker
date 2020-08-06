using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaunPacker.GUI.WPF.Common.Events
{
    /// <summary>
    /// Class representing an event that all the modules were loaded
    /// </summary>
    /// <remarks>Implements the Prism's <see cref="PubSubEvent{TPayload}"/></remarks>
    public class ModulesLoadedEvent : PubSubEvent<ModulesLoadedPayload>
    {

    }

    /// <summary>
    /// Class representing a payload of the <see cref="ModulesLoadedEvent"/>
    /// </summary>
    public class ModulesLoadedPayload
    {
        /// <summary>
        /// Constructs the payload from <paramref name="loadedTypes"/>
        /// </summary>
        /// <param name="loadedTypes">The loaded types</param>
        public ModulesLoadedPayload(IEnumerable<Type> loadedTypes)
        {
            this.loadedTypes = loadedTypes;
        }

        /// <summary>
        /// The types that were loaded and that are representing classes implementing the <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type that the returned types should implement</typeparam>
        /// <returns>The types that implement the type <typeparamref name="T"/></returns>
        public IEnumerable<Type> GetLoadedTypes<T>()
        {
            return loadedTypes.Where(x => typeof(T).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface);
        }

        private readonly IEnumerable<Type> loadedTypes;
    }
}
