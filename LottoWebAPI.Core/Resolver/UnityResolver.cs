using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

using Unity;
using Unity.Exceptions;

namespace LottoWebService.Resolver
{
    public class UnityResolver : IDependencyResolver
    {
        protected IUnityContainer container;

        public UnityResolver(IUnityContainer container)
        {
            this.container = container ?? throw new ArgumentNullException("container");
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return container.Resolve(serviceType);
            }
            catch (ResolutionFailedException exception)
            {
                return null;
            }
        }
        public T GetService<T>()
        {
            try
            {
                var serviceType = typeof(T);
                return (T)container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return default(T);
            }
        }

        public T GetService<T>(string name)
        {
            try
            {
                var serviceType = typeof(T);
                return (T)container.Resolve(serviceType, name);
            }
            catch (ResolutionFailedException)
            {
                return default(T);
            }
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            var child = container.CreateChildContainer();
            return new UnityResolver(child);
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
}