using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace BigProject.Managers
{
    /// <summary>
    /// Локатор базовых singleton служб.
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
        /// Регистрирует сервис.
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
        /// Регистрирует делегат, выдающий сервис.
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
        /// Регистрирует делегат для поиска сервиса, который будет использован только при первом обращении к сервису.
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

        /// <summary>
        /// Возвращает сервис.
        /// </summary>
        /// <returns>true если сервис найден.</returns>
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

        /// <returns>Сервис.</returns>
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
        /// Удаляет сервис. Если не найден - игнорирует.
        /// </summary>
        public static void ReleaseService<T>()
        {
            if (!_services.Remove(typeof(T)))
            {
                _resolvers.Remove(typeof(T));
            }
        }

        /// <summary>
        /// При переходах между сценами MonoBehaviour службы могут быть удалены движком.
        /// Полезно вызывать после загрузки новой сцены для удаления пустых служб.
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