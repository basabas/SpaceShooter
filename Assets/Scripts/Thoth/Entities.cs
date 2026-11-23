using System;
using System.Collections.Generic;
using System.Text;

namespace Bas.Thoth
{
    public class Entities
    {
        public int Count => _indexLookup.Count;

        //TODO: Should not be public, but I have no better solution for now. I need to be able to access this from outside the class to flush dead entities.
        public HashSet<int> RemoveList { get; private set; } = new HashSet<int>();
        public int this[int index] => _entities[index];

        private int[] _entities = new int[10_000];
        private bool[] _alive = new bool[10_000];

        private readonly Dictionary<int, int> _indexLookup = new Dictionary<int, int>();

        private Stack<int> _availableIDs = new Stack<int>();

        public int CreateEntity()
        {
            if (_availableIDs.Count == 0)
            {
                for (int i = 9_999; i >= 0; i--)
                {
                    _availableIDs.Push(i + Count);
                }
            }

            int entityID = _availableIDs.Pop();

            if (entityID >= _alive.Length)
            {
                Array.Resize(ref _alive, entityID + 1);
            }

            if (_alive[entityID])
            {
                throw new Exception($"Entity {entityID} is already alive after creation.");
            }
            _alive[entityID] = true;

            if (Count == _entities.Length)
            {
                Array.Resize(ref _entities, Count + 10_000);
            }
            _entities[Count] = entityID;
            _indexLookup[entityID] = Count;
            return entityID;
        }

        public void FlushDeadEntities()
        {
            foreach (int entityID in RemoveList)
            {
                if (_indexLookup.Remove(entityID, out int index))
                {
                    _availableIDs.Push(entityID);

                    if (Count != index)
                    {
                        _entities[index] = _entities[Count];
                        _indexLookup[_entities[index]] = index;
                    }
                }
            }
            RemoveList.Clear();
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