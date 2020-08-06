using CommonServiceLocator;
using Prism;
using Prism.Ioc;
using System;
using Unity;
using Unity.ServiceLocation;

namespace PaunPacker.GUI.Workarounds
{
    /// <summary>
    /// Workaround that was needed in order to avoid usage of Prism.Unity NuGet package (.NET Core compliance and version of Unity container used by that package)
    /// </summary>
    /// <remarks>From the Prism.Unity package, only this class was needed so it was taken and partially modified from the: https://github.com/PrismLibrary/Prism/blob/master/Source/Wpf/Prism.Unity.Wpf/PrismApplication.cs
    /// The documentation of this class can be found in Prism's documentation
    /// </remarks>
    public abstract class PrismApplication : PrismApplicationBase, IDisposable
    {
        public PrismApplication(IUnityContainer unityContainer)
        {
            UnityContainer = unityContainer;
        }
        protected override IContainerExtension CreateContainerExtension()
        {
            return new UnityContainerExtension(UnityContainer);
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(UnityContainer));
            containerRegistry.RegisterInstance(ServiceLocator.Current);
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ResolutionFailedException));
        }

        /// <summary>
        /// The unity container that should be used
        /// </summary>
        protected IUnityContainer UnityContainer { get; private set; }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    UnityContainer.Dispose();
                }

                UnityContainer = null;
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
