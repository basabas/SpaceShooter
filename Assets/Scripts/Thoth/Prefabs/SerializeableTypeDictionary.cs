using Bas.Serialization;
using Bas.Utils.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bas.Thoth
{
    [Serializable]
    public class SerializeableTypeDictionary : IDictionary<Type, AnyThing>, IEnumerable<KeyValuePair<Type, AnyThing>>
    {
        public int Count => keys.Count;

        ICollection<Type> IDictionary<Type, AnyThing>.Keys => keys.Select(key => key.Type).ToHashSet();

        ICollection<AnyThing> IDictionary<Type, AnyThing>.Values => values;

        bool ICollection<KeyValuePair<Type, AnyThing>>.IsReadOnly => true;

        public AnyThing this[Type key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [SerializeField, ClassList(typeof(Component))] private List<SerializableType> keys = new();
        [SerializeField] private List<AnyThing> values = new();

        void IDictionary<Type, AnyThing>.Add(Type key, AnyThing value) => throw new NotImplementedException();

        bool IDictionary<Type, AnyThing>.ContainsKey(Type key) => throw new NotImplementedException();

        bool IDictionary<Type, AnyThing>.Remove(Type key) => throw new NotImplementedException();

        public bool TryGetValue(Type key, out AnyThing value)
        {
            value = null;
            if (!keys.TryGetValue(t => t.Type == key, out SerializableType type))
            {
                return false;
            }

            int index = keys.IndexOf(type);
            if (index == -1)
            {
                return false;
            }

            value = values[index];
            return true;
        }

        void ICollection<KeyValuePair<Type, AnyThing>>.Add(KeyValuePair<Type, AnyThing> item) => throw new NotImplementedException();

        void ICollection<KeyValuePair<Type, AnyThing>>.Clear() => throw new NotImplementedException();

        bool ICollection<KeyValuePair<Type, AnyThing>>.Contains(KeyValuePair<Type, AnyThing> item) => throw new NotImplementedException();

        void ICollection<KeyValuePair<Type, AnyThing>>.CopyTo(KeyValuePair<Type, AnyThing>[] array, int arrayIndex) => throw new NotImplementedException();

        bool ICollection<KeyValuePair<Type, AnyThing>>.Remove(KeyValuePair<Type, AnyThing> item) => throw new NotImplementedException();

        IEnumerator<KeyValuePair<Type, AnyThing>> IEnumerable<KeyValuePair<Type, AnyThing>>.GetEnumerator()
        {
            for (int i = 0; i < keys.Count; i++)
            {
                SerializableType key = keys[i];
                AnyThing value = values[i];
                yield return new KeyValuePair<Type, AnyThing>(key, value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<Type, AnyThing>>)this).GetEnumerator();

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("[TypeDictionary]");
            for (int i = 0; i < keys.Count; i++)
            {
                SerializableType key = keys[i];
                AnyThing value = values[i];
                builder.AppendLine($"key:{key}, value:{value}");
            }
            return builder.ToString();
        }
    }
}
