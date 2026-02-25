using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace BigProject.Managers
{
    /// <summary>
    /// Locator of base singleton services.
    /// </summary>
    public static class ServiceLocator
    {
        private static Dictionary<Type, object> _services = new();
        private static Dictionary<Type, Func<object>> _resolvers = new();
        public static void Init()
        {
            _services.Clear();
            _resolvers.Clear();
        }

        /// <summary>
        /// Register service.
        /// </summary>
        public static void AddService<T>(T service)
        {
            Assert.IsTrue(service != null, "Service Locator try to add null service.");

            if (IsServiceExist<T>())
            {
                Debug.LogWarning($"Service Locator try to add service that already exist: {typeof(T).Name}");
                return;
            }

            _services.Add(typeof(T), service);
        }

        /// <summary>
        /// Registers the delegate issuing the service.
        /// </summary>
        public static void AddServiceResolver<T>(Func<T> resolver)
        {
            Assert.IsTrue(resolver != null, "Service Locator try to add null resolver.");

            if (IsServiceExist<T>())
            {
                Debug.LogWarning($"Service Locator try to add resolver that already exist: {typeof(T).Name}");
                return;
            }

            _resolvers.Add(typeof(T), () => resolver());
        }

        /// <summary>
        /// Registers a delegate for the service lookup that will only be used the first time the service is accessed.
        /// </summary>
        public static void AddServiceResolverLazy<T>(Func<T> resolver)
        {
            AddServiceResolver<T>(() =>
            {
                T service = resolver();

                if (!IsNullSerivce(service))
                {
                    _services.Add(typeof(T), service);
                    _resolvers.Remove(typeof(T));
                }

                return service;
            });
        }

        /// <returns>True when success.</returns>
        public static bool TryGetService<T>(out T service)
        {
            if (_services.TryGetValue(typeof(T), out object foundService))
            {
                service = (T)foundService;
                return !IsNullSerivce(service);
            }

            if (_resolvers.TryGetValue(typeof(T), out Func<object> foundResolver))
            {
                service = (T)foundResolver();
                return !IsNullSerivce(service);
            }

            service = default;
            return false;
        }

        public static T GetService<T>()
        {
            if (TryGetService(out T service))
            {
                return service;
            }

            return default;
        }

        public static bool IsServiceExist<T>() => _services.ContainsKey(typeof(T)) || _resolvers.ContainsKey(typeof(T));

        /// <summary>
        /// If no service with type - just ignore it.
        /// </summary>
        public static void ReleaseService<T>()
        {
            if (!_services.Remove(typeof(T)))
            {
                _resolvers.Remove(typeof(T));
            }
        }

        /// <summary>
        /// During scene transitions, MonoBehavior services may be removed by the engine.
        /// Useful to call after loading a new scene to remove empty services.
        /// </summary>
        public static void ReleaseAllEmpty()
        {
            List<Type> emptyServices = _services.Where(x => IsNullSerivce(x.Value)).Select(x => x.Key).ToList();
            
            foreach (Type serviceType in emptyServices)
            {
                _services.Remove(serviceType);
            }
        }

        private static bool IsNullSerivce(object service) => service == null || service.ToString() == "null";
    }
}