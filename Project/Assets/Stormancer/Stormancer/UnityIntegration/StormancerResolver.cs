using System;
using System.Collections.Generic;

namespace Stormancer
{
    public interface IDependencyResolver
    {
        T Resolve<T>();
        bool TryResolve<T>(out T dependency);
        //Func<IDependencyResolver, T> GetComponentFactory<T>();
        void Register<T>(Func<T> component);
        void Register<T>(Func<IDependencyResolver, T> component, bool singleInstance = false);
        void RegisterDependency<T>(T component);
    }

    public class Registration
    {
        public Func<IDependencyResolver, object> factory;
		public bool singleInstance;
        public object instance;
    };

    public class StormancerResolver : IDependencyResolver
    {
        private readonly Dictionary<Type, Registration> _registrations = new Dictionary<Type, Registration>();
        private readonly StormancerResolver _parent = null;


        public StormancerResolver(StormancerResolver parent = null)
        {
            _parent = parent;
        }


        public T Resolve<T>()
        {
            T result;
            if (TryResolve(out result))
            {
                return result;
            }
            else if(_parent.TryResolve( out result))
            {
                return result;
            }
            throw new InvalidOperationException(string.Format("The requested component of type {0} was not registered.", typeof(T)));
        }

        public bool TryResolve<T>(out T dependency)
        {
            Registration registration;
            if (_registrations.TryGetValue(typeof(T), out registration)) 
            {
                var factory = registration.factory;
                if (registration.singleInstance)
                {

                    if (registration.instance == null && factory != null)
                    {
                        registration.instance = factory(this);
                    }

                    dependency = (T)registration.instance;
                    return true;
                }
                else
                {
                    if (factory != null)
                    {
                        dependency = (T)factory(this);
                    }
                }
            }

            dependency = default(T);
            return false;
        }

        private Func<IDependencyResolver, T> ResolveFactory<T>()
        {
            Registration registration;
            if (_registrations.TryGetValue(typeof(T), out registration))
            {
                return resolver => (T)(registration.factory(resolver));
            }
            else if (_parent != null)
            {
                return _parent.ResolveFactory<T>();
            }
            else
            {
                return null;
            }
        }

        public void Register<T>(Func<T> component)
        {
            Register(c => component(), true);
        }

        public void Register<T>(Func<IDependencyResolver, T> factory, bool singleInstance = false)
        {
            Registration registration = new Registration();
            registration.factory = (dependencyResolver) => factory(dependencyResolver);
            registration.singleInstance = singleInstance;
            _registrations[typeof(T)] = registration;
        }

        public void RegisterDependency<T>(T component)
        {
            Registration registration = new Registration();
            registration.singleInstance = true;
            registration.instance = component;
            _registrations[typeof(T)] = registration;
        }
    }
}