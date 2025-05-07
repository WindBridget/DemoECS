using System;
using System.Collections.Generic;

namespace MiniECS
{
    using Entity = Int32;

    public interface IComponent { }

    public class EntityManager
    {
        private int nextId = 1;
        private readonly HashSet<Entity> entities = new();
        private readonly Dictionary<Type, Dictionary<Entity, IComponent>> pools = new();

        public Entity CreateEntity()
        {
            var id = nextId++;
            entities.Add(id);
            return id;
        }

        public void DestroyEntity(Entity e)
        {
            entities.Remove(e);
            foreach (var pool in pools.Values)
                pool.Remove(e);
        }

        public void AddComponent<T>(Entity e, T comp) where T : IComponent
        {
            var type = typeof(T);
            if (!pools.TryGetValue(type, out var pool))
                pools[type] = pool = new Dictionary<Entity, IComponent>();
            pool[e] = comp;
        }

        public bool HasComponent<T>(Entity e) where T : IComponent
            => pools.TryGetValue(typeof(T), out var p) && p.ContainsKey(e);

        public T GetComponent<T>(Entity e) where T : IComponent
            => (T)pools[typeof(T)][e];

        public IEnumerable<Entity> Query(params Type[] types)
        {
            if (types.Length == 0) yield break;
            if (!pools.TryGetValue(types[0], out var basePool)) yield break;

            foreach (var e in basePool.Keys)
            {
                bool ok = true;
                for (int i = 1; i < types.Length; i++)
                {
                    if (!pools.TryGetValue(types[i], out var p) || !p.ContainsKey(e))
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok) yield return e;
            }
        }
    }
}
