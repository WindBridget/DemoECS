using System;
using System.Collections.Generic;

namespace MiniECS
{
    using Entity = Int32;

    // Marker interface
    public interface IComponent { }

    // Pool interface exposes entity list and O(1) lookup
    interface IComponentPool
    {
        List<Entity> Entities { get; }
        bool Contains(Entity e);
        void Remove(Entity e);
    }

    // Struct-of-Arrays component pool with O(1) add/get/remove
    class ComponentPool<T> : IComponentPool where T : IComponent
    {
        private readonly List<Entity> entities = new();
        private readonly List<T> components = new();
        private readonly Dictionary<Entity, int> indexMap = new();

        public List<Entity> Entities => entities;
        public bool Contains(Entity e) => indexMap.ContainsKey(e);

        public void Add(Entity e, T comp)
        {
            if (indexMap.TryGetValue(e, out var idx))
            {
                components[idx] = comp;
            }
            else
            {
                idx = entities.Count;
                entities.Add(e);
                components.Add(comp);
                indexMap[e] = idx;
            }
        }

        public bool TryGet(Entity e, out T comp)
        {
            if (indexMap.TryGetValue(e, out var idx))
            {
                comp = components[idx];
                return true;
            }
            comp = default;
            return false;
        }

        // Amortized O(1) removal via swap-with-last
        public void Remove(Entity e)
        {
            if (!indexMap.TryGetValue(e, out var idx)) return;

            var last = entities.Count - 1;
            var lastE = entities[last];
            var lastC = components[last];

            entities[idx] = lastE;
            components[idx] = lastC;
            indexMap[lastE] = idx;

            entities.RemoveAt(last);
            components.RemoveAt(last);
            indexMap.Remove(e);
        }
    }

    // Core ECS manager
    public class EntityManager
    {
        private int nextId = 1;
        private readonly HashSet<Entity> entities = new();
        private readonly Dictionary<Type, IComponentPool> pools = new();
        private readonly List<Entity> destroyQueue = new();

        public Entity CreateEntity()
        {
            var id = nextId++;
            entities.Add(id);
            return id;
        }

        public void DestroyEntityDeferred(Entity e)
        {
            if (entities.Contains(e)) destroyQueue.Add(e);
        }

        public void FlushDestructions()
        {
            foreach (var e in destroyQueue)
                DestroyEntityImmediate(e);
            destroyQueue.Clear();
        }

        void DestroyEntityImmediate(Entity e)
        {
            if (!entities.Remove(e)) return;
            foreach (var pool in pools.Values)
                pool.Remove(e);
        }

        public void AddComponent<T>(Entity e, T comp) where T : IComponent
        {
            if (!entities.Contains(e))
                throw new InvalidOperationException($"Entity {e} does not exist.");

            if (!pools.TryGetValue(typeof(T), out var pool))
                pools[typeof(T)] = pool = new ComponentPool<T>();
            ((ComponentPool<T>)pool).Add(e, comp);
        }

        public bool HasComponent<T>(Entity e) where T : IComponent
            => pools.TryGetValue(typeof(T), out var pool) && pool.Contains(e);

        public T GetComponent<T>(Entity e) where T : IComponent
            => pools.TryGetValue(typeof(T), out var pool) && ((ComponentPool<T>)pool).TryGet(e, out var c)
               ? c
               : throw new KeyNotFoundException();

        public IEnumerable<Entity> Query(params Type[] types)
        {
            if (types.Length == 0) yield break;
            if (!pools.TryGetValue(types[0], out var basePool)) yield break;

            foreach (var e in basePool.Entities)
            {
                var ok = true;
                for (int i = 1; i < types.Length; i++)
                    if (!pools.TryGetValue(types[i], out var p) || !p.Contains(e))
                    {
                        ok = false;
                        break;
                    }
                if (ok) yield return e;
            }
        }
    }

}
