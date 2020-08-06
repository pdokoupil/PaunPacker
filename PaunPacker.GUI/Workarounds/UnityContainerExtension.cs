using Prism.Ioc;
using System;
using System.Linq;
using Unity;
using Unity.Resolution;

namespace PaunPacker.GUI.Workarounds
{
    /// <summary>
    /// The implementation of IContainerExtension for Unity container
    /// </summary>
    /// <remarks>This workaround was needed because the Unity container provided by the Prism.Unity does not offer sufficient functionality
    /// because several extension methods are missing (those from the Unity.Abstractions probably)
    /// The code in this class was taken from (and partially modified): https://github.com/PrismLibrary/Prism/blob/master/Source/Xamarin/Prism.Unity.Forms/UnityContainerExtension.cs
    /// Documentation for these methods could be found in Prism's documentation
    /// </remarks>
    class UnityContainerExtension : IContainerExtension<IUnityContainer>
    {
        public IUnityContainer Instance { get; }

        public UnityContainerExtension() : this(new UnityContainer()) { }

        public UnityContainerExtension(IUnityContainer container) => Instance = container;

        public void FinalizeExtension() { }

        public IContainerRegistry RegisterInstance(Type type, object instance)
        {
            Instance.RegisterInstance(type, instance);
            return this;
        }

        public IContainerRegistry RegisterInstance(Type type, object instance, string name)
        {
            Instance.RegisterInstance(type, name, instance);
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to)
        {
            Instance.RegisterSingleton(from, to);
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to, string name)
        {
            Instance.RegisterSingleton(from, to, name);
            return this;
        }

        public IContainerRegistry Register(Type from, Type to)
        {
            Instance.RegisterType(from, to);
            return this;
        }

        public IContainerRegistry Register(Type from, Type to, string name)
        {
            Instance.RegisterType(from, to, name);
            return this;
        }

        public object Resolve(Type type)
        {
            return Instance.Resolve(type);
        }

        public object Resolve(Type type, string name)
        {
            return Instance.Resolve(type, name);
        }

        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            var overrides = parameters.Select(p => new DependencyOverride(p.Type, p.Instance)).ToArray();
            return Instance.Resolve(type, overrides);
        }

        public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
        {
            var overrides = parameters.Select(p => new DependencyOverride(p.Type, p.Instance)).ToArray();
            return Instance.Resolve(type, name, overrides);
        }

        public bool IsRegistered(Type type)
        {
            return Instance.IsRegistered(type);
        }

        public bool IsRegistered(Type type, string name)
        {
            return Instance.IsRegistered(type, name);
        }
    }
}
