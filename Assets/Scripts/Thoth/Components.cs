using System;
using System.Collections.Generic;
using System.Linq;

namespace Bas.Thoth
{

    public class Components
    {
        private static readonly Type _componentType = typeof(Component);

        private Dictionary<Type, Component> _components = new();
        private Dictionary<Type, Component> _newComponents = new();

        private List<Component> _sortedComponents = new();

        public Component Get(Type type)
        {
            if (!_componentType.IsAssignableFrom(type))
            {
                throw new NotSupportedException($"Type {type.FullName} is not assignable from {_componentType.FullName}");
            }

            if (_components.TryGetValue(type, out Component component))
            {
                return component;
            }

            if (_newComponents.TryGetValue(type, out component))
            {
                return component;
            }

            component = (Component)Singletons.Get(type);
            _newComponents[type] = component;
            return component;
        }

        public T Get<T>() where T : Component => (T)Get(typeof(T));

        public void Update(float deltaTime)
        {
            foreach (var (type, component) in _newComponents)
            {
                _components[type] = component;
                _sortedComponents.Add(component);
            }
            if (_newComponents.Count > 0)
            {
                _sortedComponents.Sort(new ComponentPriorityComparer());
            }
            _newComponents.Clear();
            foreach (Component component in _sortedComponents)
            {
                component.Update(deltaTime);
            }
        }

        //TODO: Superslow and not efficient.
        public void FlushDeadEntities(IEnumerable<int> removeList)
        {
            foreach (Component component in _components.Values)
            {
                foreach (int entityID in removeList)
                {
                    component.RemoveEntity(entityID);
                }
            }
        }

        public void GetComponents(int entityID, List<Component> components)
        {
            foreach (Component component in _components.Values)
            {
                if (component.HasEntity(entityID))
                {
                    components.Add(component);
                }
            }
        }

        public void Dispose()
        {
            foreach (Component component in _components.Values)
            {
                if (component is IDisposable dispose)
                {
                    dispose.Dispose();
                }
            }
        }
    }

    public class ComponentPriorityComparer : IComparer<Component>
    {
        int IComparer<Component>.Compare(Component x, Component y)
        {
            if (x == null || y == null)
            {
                throw new NullReferenceException("None of the component should ever be null, what the fuck?");
            }

            PriorityAttribute priorityAttributeX = x.GetType().GetCustomAttributes(typeof(PriorityAttribute), true).FirstOrDefault() as PriorityAttribute;
            PriorityAttribute priorityAttributeY = y.GetType().GetCustomAttributes(typeof(PriorityAttribute), true).FirstOrDefault() as PriorityAttribute;

            if (priorityAttributeX == null)
            {
                priorityAttributeX = new PriorityAttribute(Priority.Rest);
            }
            if (priorityAttributeY == null)
            {
                priorityAttributeY = new PriorityAttribute(Priority.Rest);
            }
            if (priorityAttributeX.Priority == priorityAttributeY.Priority)
            {
                return 1;
            }
            return priorityAttributeX.Priority.CompareTo(priorityAttributeY.Priority);
        }
    }

    public class MissingAttributeException : Exception
    {
        public MissingAttributeException(string message) : base(message)
        {
        }
    }

    public class PriorityAttribute : Attribute
    {
        public int Priority { get; private set; }
        public PriorityAttribute(Priority priority)
        {
            Priority = (int)priority;
        }

        public PriorityAttribute(int priority)
        {
            Priority = Math.Clamp(priority, 2, 254);
        }
    }

    public enum Priority
    {
        Physics = 0,
        Collisions = 1,
        Rest = 2,
        Graphics = 255
    }
}