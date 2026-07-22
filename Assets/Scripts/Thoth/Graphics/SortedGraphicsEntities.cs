using System;
using System.Collections.Generic;
using System.Text;

namespace Bas.Thoth
{
    public class SortedGraphicsEntities
    {
        public int[] Entities => _entities;
        public int Count { get; private set; }

        private readonly Dictionary<int, int> _indexLookup = new();

        private readonly int[] _typeStarts;

        private readonly Graphics _graphics;
        public int[] _entities;

        public SortedGraphicsEntities(Graphics graphics, int prefabCount)
        {
            _graphics = graphics;
            _entities = new int[10_000];
            _typeStarts = new int[prefabCount];
        }

        public void AddEntity(int entityID)
        {
            if (Count == _entities.Length - 1)
            {
                Array.Resize(ref _entities, _entities.Length + 10_000);
            }

            int textureIndex = _graphics[entityID].textureIndex;
            int index = Count++;

            for (int type = 0; type != textureIndex; type++)
            {
                int typeStart = _typeStarts[type];
                _typeStarts[type] = typeStart + 1;
                if (type > 0 && _typeStarts[type] == _typeStarts[type - 1])
                {
                    continue;
                }

                int id = _entities[typeStart];

                _indexLookup[id] = index;
                _entities[index] = id;


                index = typeStart;
            }

            _indexLookup[entityID] = index;
            _entities[index] = entityID;
        }

        public void RemoveEntity(int entityID)
        {
            if (_indexLookup.Remove(entityID, out int index))
            {
                Count--;
                for (int type = _graphics[entityID].textureIndex - 1; type > -1; type--)
                {
                    int typeEnd = _typeStarts[type] - 1;
                    _typeStarts[type] = typeEnd;
                    if (type > 0 && typeEnd == _typeStarts[type - 1] - 1)
                    {
                        continue;
                    }

                    int id = _entities[typeEnd];
                    _indexLookup[id] = index;
                    _entities[index] = id;

                    index = typeEnd;
                }

                int lastId = _entities[Count];
                _indexLookup[lastId] = index;
                _entities[index] = lastId;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"SortedEntities - Count:{Count}, entities.Length:{_entities.Length}");

            for (int i = 0; i < Count; i++)
            {
                int id = _entities[i];
                builder.AppendLine($"{i} - {_graphics[id].textureIndex}: id {id}, lookup:{(_indexLookup.TryGetValue(id, out int index) ? index : -1)}");
            }
            return builder.ToString();
        }
    }
}