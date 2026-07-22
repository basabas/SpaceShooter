using Bas.Serialization;
using System;
using UnityEngine;

namespace Bas.Thoth
{
    [Serializable]
    public class AnyThing
    {
        public static AnyThing Empty => new AnyThing { json = "nothing", type = new SerializableType("nothing") };

        [SerializeField] private SerializableType type = new(typeof(AnyThing));
        [SerializeField] private string json;

        public T As<T>()
        {
            if (type.Type != typeof(T))
            {
                throw new Exception($"Wrong type, expected {type.Type.FullName} but got {typeof(T).FullName} with json:{json}");
            }
            return JsonUtility.FromJson<T>(json);
        }

        public void Set<T>(T o)
        {
            type.Type = typeof(T);
            json = ToJson(o);
        }

        public static string ToJson(object o) => JsonUtility.ToJson(o);
        public static object FromJson(string json, Type type) => JsonUtility.FromJson(json, type);

        public override string ToString() => $"[Anything] Type:{type}, json:{json}";
    }
}
