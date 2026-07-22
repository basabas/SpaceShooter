using System.Collections;
using System.Collections.Generic;

namespace Bas.Utils
{
    public class HashedLinkedList<T> : IEnumerable<T>
    {
        private readonly LinkedList<T> _linkedList = new LinkedList<T>();
        private readonly Dictionary<T, LinkedListNode<T>> _dictionary = new Dictionary<T, LinkedListNode<T>>();

        public LinkedListNode<T> First => _linkedList.First;

        public LinkedListNode<T> Last => _linkedList.Last;

        public void Add(T item)
        {
            if (_dictionary.ContainsKey(item))
            {
                LinkedListNode<T> node = _linkedList.AddLast(item);
                _dictionary.Add(item, node);
            }
        }

        public void Remove(T item)
        {
            if (_dictionary.TryGetValue(item, out LinkedListNode<T> node))
            {
                _dictionary.Remove(item);
                _linkedList.Remove(node);
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _linkedList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _linkedList.GetEnumerator();
    }
}
