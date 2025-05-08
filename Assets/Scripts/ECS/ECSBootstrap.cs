using UnityEngine;
using MiniECS;
using MiniECS.Components;
using MiniECS.Systems;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class ECSBootstrap : MonoBehaviour
{
    [Header("Demo Settings")]
    [SerializeField] int SpawnCount = 5000;
    [SerializeField] float Spread = 20f;
    [SerializeField] float Speed = 5f;
    [SerializeField] float Lifetime = 10f;
    [Header("Render Settings")]
    [SerializeField] Mesh BulletMesh;
    [SerializeField] Material BulletMaterial;

    EntityManager em;
    List<ISystem> systems;

    ISystem renderSystem;

    void Start()
    {
        em = new EntityManager();
        systems = new List<ISystem>
        {
            new MovementSystemECS(),
            new CleanupSystemECS(),      // ← xóa theo khoảng cách trước
            new LifetimeSystemECS(),     // ← đánh dấu hết life → add BulletTagECS
            new BulletDestroySystemECS(),// ← chỉ destroy deferred
        };

        renderSystem = new RenderSystemECS();

        BulletEntityECS.Initialize(em, BulletMesh, BulletMaterial);
    }

    public void Spawn()
    {
        for (int i = 0; i < SpawnCount; i++)
        {
            var pos = new Vector3(
                Random.Range(-Spread, Spread),
                0f,
                Random.Range(-Spread, Spread)
            );

            var dir = new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)
            ).normalized;

            BulletEntityECS.Spawn(pos, dir, Speed, Lifetime);
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;
        foreach (var sys in systems)
            sys.Update(em, dt);

        em.FlushDestructions();
        renderSystem.Update(em, dt);
    }
}
