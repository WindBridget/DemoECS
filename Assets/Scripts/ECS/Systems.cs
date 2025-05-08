using System.Collections.Generic;
using MiniECS;
using MiniECS.Components;
using UnityEngine;

namespace MiniECS.Systems
{
    public interface ISystem { void Update(EntityManager em, float dt); }

    public class MovementSystemECS : ISystem
    {
        public void Update(EntityManager em, float dt)
        {
            // Snapshot entities để tránh sửa collection khi enumerate
            var ents = new List<int>();
            foreach (var e in em.Query(typeof(PositionECS), typeof(VelocityECS)))
                ents.Add(e);

            foreach (var e in ents)
            {
                var p = em.GetComponent<PositionECS>(e);
                var v = em.GetComponent<VelocityECS>(e);
                p.X += v.VX * dt;
                p.Z += v.VZ * dt;
                em.AddComponent(e, p);
            }
        }
    }

    public class LifetimeSystemECS : ISystem
    {
        public void Update(EntityManager em, float dt)
        {
            var ents = new List<int>();
            foreach (var e in em.Query(typeof(LifetimeECS)))
                ents.Add(e);

            foreach (var e in ents)
            {
                var life = em.GetComponent<LifetimeECS>(e);
                life.Remaining -= dt;
                if (life.Remaining <= 0f)
                    em.AddComponent(e, new BulletTagECS());
                else
                    em.AddComponent(e, life);
            }
        }
    }

    public class BulletDestroySystemECS : ISystem
    {
        // reuse buffer
        readonly List<int> buffer = new List<int>();

        public void Update(EntityManager em, float dt)
        {
            buffer.Clear();
            foreach (var e in em.Query(typeof(BulletTagECS), typeof(PositionECS)))
                buffer.Add(e);

            for (int i = 0, c = buffer.Count; i < c; i++)
                em.DestroyEntityDeferred(buffer[i]);
        }
    }

    public class CleanupSystemECS : ISystem
    {
        const float Limit = 50f;
        // reuse buffer mỗi frame
        readonly List<int> buffer = new List<int>();

        public void Update(EntityManager em, float dt)
        {
            buffer.Clear();
            // lấy hết entity có Position để test khoảng cách
            foreach (var e in em.Query(typeof(PositionECS)))
                buffer.Add(e);

            for (int i = 0, c = buffer.Count; i < c; i++)
            {
                int e = buffer[i];
                var p = em.GetComponent<PositionECS>(e);
                if (Mathf.Abs(p.X) > Limit || Mathf.Abs(p.Z) > Limit)
                {
                    em.DestroyEntityDeferred(e);
                }
            }
        }
    }

    public class FlushSystemECS : ISystem
    {
        public void Update(EntityManager em, float dt)
        {
            em.FlushDestructions();
        }
    }

    public class RenderSystemECS : ISystem
    {
        // Batch by mesh & material
        private readonly List<int> queryList = new List<int>();
        private readonly Dictionary<(Mesh mesh, Material mat), List<Matrix4x4>> batches
            = new Dictionary<(Mesh, Material), List<Matrix4x4>>();

        public void Update(EntityManager em, float dt)
        {
            batches.Clear();
            queryList.Clear();
            queryList.AddRange(em.Query(typeof(PositionECS), typeof(MeshECS), typeof(MaterialECS)));

            // Collect matrices
            foreach (var e in queryList)
            {
                var p = em.GetComponent<PositionECS>(e);
                var meshComp = em.GetComponent<MeshECS>(e);
                var matComp = em.GetComponent<MaterialECS>(e);
                var key = (meshComp.Mesh, matComp.Mat);

                if (!batches.TryGetValue(key, out var mats))
                {
                    mats = new List<Matrix4x4>();
                    batches[key] = mats;
                }

                Vector3 pos = new Vector3(p.X, 0f, p.Z);
                mats.Add(Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one * 0.1f));
            }

            // Draw each batch once
            foreach (var kv in batches)
            {
                var mesh = kv.Key.Item1;    // <-- đây
                var mat = kv.Key.Item2;    // <-- và đây
                var mats = kv.Value;

                if (!mat.enableInstancing)
                    mat.enableInstancing = true;

                Graphics.DrawMeshInstanced(mesh, 0, mat, mats);
            }
        }
    }

}