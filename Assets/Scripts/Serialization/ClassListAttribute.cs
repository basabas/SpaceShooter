using Bas.Utils.Reflection;
using System;
using System.Collections.Generic;

namespace Bas.Serialization
{
    public class ClassListAttribute : SerializableTypeAttribute
    {
        public override IEnumerable<Type> FilteredTypes => ReflectionTools.GetDerivedTypes(AppDomain.CurrentDomain.GetAssemblies(), _type);

        private readonly Type _type;

        public ClassListAttribute(Type type, SerializableTypeOptions options = SerializableTypeOptions.None) : base(options & ~SerializableTypeOptions.IncludePrimitives) => _type = type;
    }
}