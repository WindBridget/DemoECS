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
    [SerializeField] float Spread     = 20f;
    [SerializeField] float Speed      = 5f;
    [SerializeField] float Lifetime   = 10f;
    [Header("Render Settings")]
    [SerializeField] Mesh    BulletMesh;
    [SerializeField] Material BulletMaterial;

    EntityManager em;
    List<ISystem> systems;

    void OnEnable()
    {
        em = new EntityManager();
        systems = new List<ISystem>
        {
            new MovementSystemECS(),
            new LifetimeSystemECS(),
            new BulletDestroySystemECS(),
            new RenderSystemECS()
        };

        for (int i = 0; i < SpawnCount; i++)
        {
            var e = em.CreateEntity();
            float x = Random.Range(-Spread, Spread);
            float y = Random.Range(-Spread, Spread);
            em.AddComponent(e, new PositionECS { X = x, Y = y });

            var dir = new Vector2(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;
            em.AddComponent(e, new VelocityECS { VX = dir.x * Speed, VY = dir.y * Speed });

            em.AddComponent(e, new LifetimeECS { Remaining = Lifetime });

            // assign mesh & material
            em.AddComponent(e, new MeshECS { Mesh = BulletMesh });
            em.AddComponent(e, new MaterialECS { Mat = BulletMaterial });
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;
        foreach (var sys in systems)
            sys.Update(em, dt);
    }
}
