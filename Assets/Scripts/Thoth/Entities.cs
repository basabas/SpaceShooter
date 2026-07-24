using System;
using System.Collections.Generic;
using System.Text;

namespace Bas.Thoth
{
    public class Entities
    {
        private int[] _entities = new int[10_000];
        private bool[] _alive = new bool[10_000];

        private readonly Dictionary<int, int> _indexLookup = new Dictionary<int, int>();

        private Stack<int> _availableIDs = new Stack<int>();

        public HashSet<int> RemoveList { get; private set; } = new HashSet<int>();

        public int Count { get; private set; }

        public int CreateEntity()
        {
            if (_availableIDs.Count == 0)
            {
                for (int i = 9_999; i >= 0; i--)
                {
                    _availableIDs.Push(i + Count);
                }
            }

            int id = _availableIDs.Pop();

            if (id >= _alive.Length)
            {
                Array.Resize(ref _alive, id + 1);
            }
            _alive[id] = true;

            if (Count == _entities.Length - 1)
            {
                Array.Resize(ref _entities, Count + 10_000);
            }
            _entities[Count] = id;
            _indexLookup[Count] = id;
            Count++;

            return id;
        }

        public void FlushDeadEntities()
        {
            foreach (int id in RemoveList)
            {
                RemoveEntity(id);
            }
            RemoveList.Clear();
        }

        private void RemoveEntity(int entityID)
        {
            if (_indexLookup.Remove(entityID, out int index))
            {
                Count--;
                int newId = _entities[Count];
                _indexLookup[newId] = index;
                _entities[index] = newId;
            }
        }

        public void DestroyEntity(int entityID)
        {
            _alive[entityID] = false;
            RemoveList.Add(entityID);
        }

        public bool IsAlive(int entityID)
        {
            if (entityID < 0 || entityID >= _alive.Length)
            {
                throw new IndexOutOfRangeException($"Entity ID {entityID} is out of range. Valid range is 0 to {_alive.Length - 1}.");
            }
            return _alive[entityID];
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Entities - Count:{Count}, entities.Length:{_entities.Length}");

            for (int i = 0; i < Count; i++)
            {
                int id = _entities[i];
                builder.AppendLine($"{i} - id {id}, lookup:{(_indexLookup.TryGetValue(id, out int index) ? index : -1)}");
            }
            return builder.ToString();
        }
    }
}