using Bas.Debug;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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
                    #region something goes wrong here, need to figure out how to fix it
                    int typeEnd = _typeStarts[type] - 1;
                    _typeStarts[type] = typeEnd;
                    if (type > 0 && typeEnd == _typeStarts[type - 1] - 1)
                    {
                        continue;
                    }
                    int id = _entities[typeEnd];
                    if (id != entityID)
                    {
                        _indexLookup[id] = index;
                        _entities[index] = id;
                    }

                    index = typeEnd;
                    #endregion
                }

                int lastId = _entities[Count];
                _indexLookup[lastId] = index;
                _entities[index] = lastId;
            }
            else
            {
                Logging.LogError($"Entity {entityID} not found in SortedGraphicsEntities. Current Count: {Count}\n{this}");
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"SortedEntities - Count:{Count}, indexLookup.Count:{_indexLookup.Count}, entities.Length:{_entities.Length}");

            string[] textureNames = new string[_typeStarts.Length * 20];

            int textureNameLength = 1;
            for (int i = 0; i < textureNames.Length; i++)
            {
                textureNames[i] = Resources.Load<PrefabManager>("PrefabManager").GetPrefabName(i);
                if (textureNames[i].Length > textureNameLength)
                {
                    textureNameLength = textureNames[i].Length;
                }
            }

            for (int i = 0; i < textureNames.Length; i++)
            {
                if (textureNames[i].Length < textureNameLength)
                {
                    int lengthToPad = textureNameLength - textureNames[i].Length;
                    textureNames[i] = textureNames[i].PadRight(lengthToPad) + "\t";
                }
            }

            for (int i = 0; i < Count; i++)
            {
                int id = _entities[i];
                builder.AppendLine($"{i:000000} - {_graphics[id].textureIndex:000} - {textureNames[_graphics[id].textureIndex]}: id {id:000000}\t, lookup:{(_indexLookup.TryGetValue(id, out int index) ? index : -1)}");
            }
            return builder.ToString();
        }
    }
}