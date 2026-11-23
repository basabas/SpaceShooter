using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Bas.Thoth
{
    public abstract class Component : IInit
    {
        private readonly Dictionary<int, int> _indexLookup = new Dictionary<int, int>();
        protected int Count => _indexLookup.Count;

        private int[] _entities = new int[10_000];

        public virtual void AddEntity(int entityID)
        {
            if (!_indexLookup.TryAdd(entityID, Count))
            {
                throw new Exception($"[{GetType().Name}]Collection already contains id:{entityID}");
            }
            if (_entities.Length == Count)
            {
                Array.Resize(ref _entities, _entities.Length + 10_000);
            }
            _entities[Count - 1] = entityID;
        }

        public virtual void Init() { }

        public virtual bool RemoveEntity(int entityID)
        {
            if (!_indexLookup.Remove(entityID, out int index))
            {
                return false;
            }
            if (Count != index)
            {
                _entities[index] = _entities[Count];
                _indexLookup[_entities[index]] = index;
            }

            return true;
        }

        public virtual void Update(float deltaTime)
        {
            ReadOnlySpan<int> span = _entities.AsSpan();

            ref int current = ref MemoryMarshal.GetReference(span);
            ref int end = ref Unsafe.Add(ref current, Count);

            // Direct pointer-like walk without indexing calculations
            while (Unsafe.IsAddressLessThan(ref current, ref end))
            {
                // Process the data directly via reference
                UpdateEntity(current, deltaTime);

                // Move the reference to the next element
                current = ref Unsafe.Add(ref current, 1);
            }
        }

        public abstract void UpdateEntity(int entityID, float deltaTime);

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"{GetType().Name} - Count:{Count}, entities.Length:{_entities.Length}");

            for (int i = 0; i < Count; i++)
            {
                int id = _entities[i];
                builder.AppendLine($"{i} - id {id}, lookup:{(_indexLookup.TryGetValue(id, out int index) ? index : -1)}");
            }
            return builder.ToString();
        }

        public bool HasEntity(int entityID) => _indexLookup.ContainsKey(entityID);
    }

    public abstract class Component<T> : Component, ICanSetAnything where T : struct
    {
        public T[] Values = new T[10_000];

        public T this[int entityID]
        {
            set => Values[entityID] = value;
            get => Values[entityID];
        }

        public override void AddEntity(int entityID)
        {
            while (entityID >= Values.Length)
            {
                Array.Resize(ref Values, Values.Length + 10_000);
            }
            Values[entityID] = new T();
            base.AddEntity(entityID);
        }

        void ICanSetAnything.SetValue(int entityID, AnyThing anything) => this[entityID] = anything.As<T>();

        public void UpdateValue(int entityID, Func<T, T> updateFunction) => Values[entityID] = updateFunction(Values[entityID]);

#if CSHARP_10_OR_NEWER
        public void TEST<T>(int entityID, Func<ref T> updateFunction) where T : allows ref struct
        {
        //TODO, make this
            Values[entityID] = updateFunction(Values[entityID]);
        }
#endif
    }

    public interface ICanSetAnything
    {
        void SetValue(int entityID, AnyThing anything);
    }
}
