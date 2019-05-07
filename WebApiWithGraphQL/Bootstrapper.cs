using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using WebApiWithGraphQL.Data;
using WebApiWithGraphQL.GraphQL;

namespace WebApiWithGraphQL
{
    public interface IServiceContainer : IDisposable
    {
        object Get(Type serviceType);
        T Get<T>();
        void Register<TService>();
        void Register<TService>(Func<TService> instanceCreator);
        void Register<TService, TImpl>() where TImpl : TService;
        void Singleton<TService>(TService instance);
        void Singleton<TService>(Func<TService> instanceCreator);
    }

    public class Bootstrapper
    {
        public IServiceContainer container { get; set; }

        public Bootstrapper()
        {
            container = BuildContainer();
        }

        public IDependencyResolver Resolver()
        {
            var resolver = new ServiceContainerDependencyResolver(container);
            return resolver;
        }
        private IServiceContainer BuildContainer()
        {
            var container = new ServiceContainer();
            container.Singleton<IDocumentExecuter>(new DocumentExecuter());
            container.Singleton<IDocumentWriter>(new DocumentWriter(true));
            container.Singleton(new StarWarsData());
            container.Register<StarWarsQuery>();
            container.Register<HumanType>();
            container.Register<DroidType>();
            container.Register<CharacterInterface>();
            container.Singleton(new StarWarsSchema(type => (GraphType)container.Get(type)));
            return container;
        }

        protected class ServiceContainer : IServiceContainer
        {
            private readonly Dictionary<Type, Func<object>> _registrations = new Dictionary<Type, Func<object>>();
            public void Register<TService>()
            {
                Register<TService, TService>();
            }
            public void Register<TService, TImpl>() where TImpl : TService
            {
                _registrations.Add(typeof(TService),
                    () =>
                    {
                        var implType = typeof(TImpl);
                        return typeof(TService) == implType
                            ? CreateInstance(implType)
                            : Get(implType);
                    });
            }
            public void Register<TService>(Func<TService> instanceCreator)
            {
                _registrations.Add(typeof(TService), () => instanceCreator());
            }
            public void Singleton<TService>(TService instance)
            {
                _registrations.Add(typeof(TService), () => instance);
            }
            public void Singleton<TService>(Func<TService> instanceCreator)
            {
                var lazy = new Lazy<TService>(instanceCreator);
                Register(() => lazy.Value);
            }
            public T Get<T>()
            {
                return (T)Get(typeof(T));
            }
            public object Get(Type serviceType)
            {
                if (_registrations.TryGetValue(serviceType, out var creator))
                {
                    return creator();
                }
                if (!serviceType.IsAbstract)
                {
                    return CreateInstance(serviceType);
                }
                throw new InvalidOperationException("No registration for " + serviceType);
            }
            public void Dispose()
            {
                _registrations.Clear();
            }
            private object CreateInstance(Type implementationType)
            {
                var ctor = implementationType.GetConstructors().OrderByDescending(x => x.GetParameters().Length).First();
                var parameterTypes = ctor.GetParameters().Select(p => p.ParameterType);
                var dependencies = parameterTypes.Select(Get).ToArray();
                return Activator.CreateInstance(implementationType, dependencies);
            }
        }
    }



    public class ServiceContainerDependencyResolver : IDependencyResolver
    {
        private readonly IServiceContainer _container;
        public ServiceContainerDependencyResolver(IServiceContainer container)
        {
            _container = container;
        }
        public object GetService(Type serviceType)
        {
            try
            {
                return _container.Get(serviceType);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Enumerable.Empty<object>();
        }
      
        public void Dispose()
        {
        }

        public T Resolve<T>()
        {
            try
            {
                return (T)_container.Get(typeof(T));
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public object Resolve(Type type)
        {
            try
            {
                return _container.Get(type);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

