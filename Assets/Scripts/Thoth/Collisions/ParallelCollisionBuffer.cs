using System;
using System.Collections;
using System.Collections.Generic;

namespace Bas.Thoth
{
    public class ParallelCollisionBuffer : IEnumerable<CollisionResult>
    {
        public int Count
        {
            get
            {
                int count = 0;
                for (int i = 0; i < results.Length; i++)
                {
                    count += results[i].Count;
                }
                return count;
            }
        }

        public List<CollisionResult>[] results;

        public ParallelCollisionBuffer(int size)
        {
            results = new List<CollisionResult>[size];
            for (int i = 0; i < size; i++)
            {
                results[i] = new();
            }
        }

        public void Resize(int size)
        {
            int length = results.Length;
            if (size > results.Length)
            {
                Array.Resize(ref results, size);
            }
            for (int i = length; i < size; i++)
            {
                results[i] = new();
            }
        }

        public void Clear()
        {
            for (int i = 0; i < results.Length; i++)
            {
                results[i].Clear();
            }
        }

        public void Add(int threadIndex, in CollisionResult result) => results[threadIndex].Add(result);

        public IEnumerator<CollisionResult> GetEnumerator()
        {
            for (int i = 0; i < results.Length; i++)
            {
                for (int j = 0; j < results[i].Count; j++)
                {
                    yield return results[i][j];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}