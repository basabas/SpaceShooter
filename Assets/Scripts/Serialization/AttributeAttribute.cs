using Bas.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bas.Serialization
{
    public class AttributeAttribute : SerializableTypeAttribute
    {
        public override IEnumerable<Type> FilteredTypes =>
            ReflectionTools.GetDerivedTypes(AppDomain.CurrentDomain.GetAssemblies(), typeof(object))
                .Where(type => type.CustomAttributes.Any(a => a.AttributeType == _attributeType));

        private readonly Type _attributeType;

        public AttributeAttribute(Type attributeType, SerializableTypeOptions options = SerializableTypeOptions.None) : base(options & ~SerializableTypeOptions.IncludePrimitives)
        {
            if (typeof(Attribute).IsAssignableFrom(attributeType))
            {
                _attributeType = attributeType;
            }
            else
            {
                Debug.LogError($"{nameof(attributeType)} should be derived from {nameof(Attribute)}");
            }
        }
    }
}
