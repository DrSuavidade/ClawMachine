#nullable enable
using System;
using System.Collections.Generic;

namespace Project.Core
{
    /// <summary>Simple service locator for small projects. Replace with DI if needed.</summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Register<T>(T instance) where T : class
        {
            _services[typeof(T)] = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        public static T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var obj))
                return (T)obj;
            throw new InvalidOperationException($"Service not registered: {typeof(T).Name}");
        }

        public static bool TryGet<T>(out T? service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var obj))
            {
                service = (T)obj;
                return true;
            }
            service = null;
            return false;
        }
    }
}
