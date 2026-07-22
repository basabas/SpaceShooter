using Bas.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bas.Serialization
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class SerializableTypeAttribute : PropertyAttribute
    {
        public SerializableTypeOptions Options { get; }

        /// <summary>
        /// All <see cref="Type"/>s used in the options dropdown.
        /// </summary>
        public virtual IEnumerable<Type> FilteredTypes => ReflectionTools
            .GetDerivedTypes(ReflectionTools.GetUnityProjectAssemblies(), typeof(object), true, true)
            .Where(type => !type.FullName.Contains("+<>c"));

        public SerializableTypeAttribute(SerializableTypeOptions options = SerializableTypeOptions.None) => Options = options;
    }
}