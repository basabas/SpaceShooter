using Bas.Debug;
using Bas.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bas.Thoth
{
    public static class Singletons
    {
        private static readonly Type _componentType = typeof(UnityEngine.Component);
        private static readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();

        public static T Get<T>() where T : class, new() => (T)Get(typeof(T));

        public static object Get(Type type)
        {
            if (_singletons.TryGetValue(type, out object singleton))
            {
                return singleton;
            }

            singleton = _componentType.IsAssignableFrom(type) ? CreateComponent(type) : Activator.CreateInstance(type);

            if (singleton is IInit init)
            {
                init.Init();
            }

            SetSingleton(singleton);
            return singleton;
        }

        public static object GetInstanceOf(Type type)
        {
            IEnumerable<Type> derivedTypes = ReflectionTools.GetDerivedTypes(type);

            if (derivedTypes.Count() == 0)
            {
                Logging.LogError($"Could not find an instance of {type.FullName}");
                return null;
            }
            else if (derivedTypes.Count() > 1)
            {
                Logging.LogWarning($"Found more than 1 instance of {type.FullName}: {string.Join(',', derivedTypes.Select(t => t.Name))}. Choosing the first:{derivedTypes.First().Name}");
            }

            return Get(derivedTypes.First());
        }

        public static T GetInstanceOf<T>() where T : class => (T)GetInstanceOf(typeof(T));

        private static UnityEngine.Component CreateComponent(Type type) => new UnityEngine.GameObject(type.Name, type).GetComponent(type);

        internal static bool SingletonExists(Type type) => _singletons.ContainsKey(type);

        internal static void SetSingleton(object singleton) => _singletons.Add(singleton.GetType(), singleton);

        internal static void RemoveSingleton(Type type) => _singletons.Remove(type);
    }

    public interface IInit
    {
        void Init();
    }
}
