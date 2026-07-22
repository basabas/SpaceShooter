using System;
using System.Reflection;
using UnityEngine;

namespace Bas.Serialization
{
    [Serializable]
    public class SerializableType
    {
        [SerializeField] private string typeName;
        [SerializeField] private string assemblyName;

        private Type _type;
        private Assembly _assembly;

        public Type Type
        {
            get
            {
                if(_assembly == null || _assembly.FullName != assemblyName)
                {
                    _assembly = Assembly.Load(assemblyName);
                }
                if (_type == null || _type.FullName != typeName)
                {
                    _type = _assembly?.GetType(typeName);
                }
                return _type;
            }
            set
            {
                _type = value;
                assemblyName = value.Assembly.FullName;
                typeName = value.FullName;
            }
        }

        public SerializableType(Type type) => Type = type;

        public SerializableType(string typeFullName) => Type = Type.GetType(typeFullName);

        public static implicit operator Type(SerializableType type) => type.Type;

        public override string ToString() => $"[SerializableType] {typeName}";
    }
}