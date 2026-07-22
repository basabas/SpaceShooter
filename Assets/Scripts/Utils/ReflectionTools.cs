using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Assembly = System.Reflection.Assembly;

namespace Bas.Utils.Reflection
{
    public static class ReflectionTools
    {
        private static Assembly[] Assemblies
        {
            get { return _assemblies ??= AppDomain.CurrentDomain.GetAssemblies(); }
        }

        private static Assembly[] _assemblies;

        public static IEnumerable<Type> GetDerivedTypes<T>(bool allowInterfaces = false, bool allowAbstracts = false)
        {
            return GetDerivedTypes(Assemblies, typeof(T), allowInterfaces, allowAbstracts);
        }

        public static IEnumerable<Type> GetDerivedTypes(Type type, bool allowInterfaces = false, bool allowAbstracts = false)
        {
            return GetDerivedTypes(Assemblies, type, allowInterfaces, allowAbstracts);
        }

        public static IEnumerable<Type> GetDerivedTypes(IEnumerable<Assembly> assemblies, Type baseType, bool allowInterfaces = false, bool allowAbstracts = false)
        {
            List<Type> derivedTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (baseType.IsAssignableFrom(type) && (!type.IsInterface || allowInterfaces) && ((!type.IsAbstract || allowAbstracts) && !type.IsSealed))
                        {
                            derivedTypes.Add(type);
                        }
                    }
                }
                catch (Exception exc)
                {
                    Debug.LogError(exc);
                }
            }

            return derivedTypes;
        }

        public static IEnumerable<T> GetDerivedInstances<T>(IEnumerable<Type> derivedTypes)
        {
            return derivedTypes.Select(type => (T)Activator.CreateInstance(type));
        }

        public static IEnumerable<T> GetDerivedInstances<T>(Assembly[] assemblies)
        {
            return GetDerivedInstances<T>(GetDerivedTypes(assemblies, typeof(T)));
        }

        public static IEnumerable<T> GetDerivedInstances<T>()
        {
            return GetDerivedInstances<T>(GetDerivedTypes(Assemblies, typeof(T)));
        }

        public static IEnumerable<Assembly> GetUnityProjectAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>();
#if UNITY_EDITOR
            foreach (string assetPath in AssetDatabase.GetAllAssetPaths())
            {
                Type type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                if (type is { Name: "AssemblyDefinitionAsset" } && TryGetAssembly(Path.GetFileNameWithoutExtension(assetPath), out Assembly assembly))
                {
                    assemblies.Add(assembly);
                }
            }
#endif//UNITY_EDITOR

            return assemblies;
        }

        public static IEnumerable<Assembly> GetAssemblies(params string[] assemblyNames)
        {
            List<Assembly> assemblies = new List<Assembly>();
            foreach (string assemblyName in assemblyNames)
            {
                if (TryGetAssembly(assemblyName, out Assembly assembly))
                {
                    assemblies.Add(assembly);
                }
            }

            return assemblies;
        }

        public static IEnumerable<Type> GetAllTypes()
        {
            return GetAllTypes(Assemblies);
        }

        public static IEnumerable<Type> GetAllTypes(IEnumerable<Assembly> assemblies)
        {
            List<Type> types = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        types.Add(type);
                    }
                }
                catch (Exception exc)
                {
                    Debug.LogError(exc);
                }
            }

            return types;
        }

        public static bool TryGetAssembly(string name, out Assembly assembly)
        {
            foreach (Assembly ass in Assemblies)
            {
                if (ass.FullName.Substring(0, ass.FullName.IndexOf(",", StringComparison.Ordinal)).Equals(name))
                {
                    assembly = ass;
                    return true;
                }
            }

            assembly = null;
            return false;
        }

        public static bool TryGetType(string typeFullName, string assemblyFullName, out Type type)
        {
            Assembly assembly;
            type = null;
            try
            {
                assembly = Assembly.Load(assemblyFullName);
            }
            catch (Exception)
            {
                Debug.LogWarning($"Could not find Assembly:{assemblyFullName}, trying bruteforce");
                if (!TryGetAssembly(assemblyFullName, out assembly))
                {
                    Debug.LogWarning($"Could not find Assembly:{assemblyFullName}, if with bruteforce");
                    return false;
                }
            }
            type = assembly.GetType(typeFullName);

            if (type == null)
            {
                throw new WrongAssemblyException($"Could not find type:{typeFullName} in assembly:{assemblyFullName}");
            }
            return true;
        }

        public static bool TryGetType(string typeFullName, out Type type)
        {
            foreach (Assembly assembly in Assemblies)
            {
                type = assembly.GetType(typeFullName);
                if (type != null)
                {
                    return true;
                }
            }

            type = null;
            return false;
        }
    }

    public class WrongAssemblyException : Exception
    {
        public WrongAssemblyException(string message) : base(message) { }
    }
}
