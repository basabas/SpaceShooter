using System.Collections.Generic;

namespace Bas.Thoth
{
    public class OverlapHandler
    {
        public Dictionary<int, HashSet<IOnCollision>> subscribers = new Dictionary<int, HashSet<IOnCollision>>();

        public void HandleTrigger(CollisionResult collisionResult)
        {
            if (subscribers.TryGetValue(collisionResult.id, out HashSet<IOnCollision> result))
            {
                foreach (IOnCollision subscriber in result)
                {
                    subscriber.OnCollision(collisionResult);
                }
            }
            if (subscribers.TryGetValue(collisionResult.otherId, out result))
            {
                foreach (IOnCollision subscriber in result)
                {
                    subscriber.OnCollision(collisionResult);
                }
            }
        }

        public void Subscribe(int entityID, IOnCollision subscriber)
        {
            if (!subscribers.TryGetValue(entityID, out HashSet<IOnCollision> result))
            {
                subscribers[entityID] = result = new HashSet<IOnCollision>();
            }
            result.Add(subscriber);
        }

        public void UnSubscribe(int entityID, IOnCollision subscriber)
        {
            if (subscribers.TryGetValue(entityID, out HashSet<IOnCollision> result))
            {
                result.Remove(subscriber);
            }
        }
    }
}