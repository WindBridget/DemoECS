// File: BulletFactory.cs
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public static class BulletEntity
{
    static EntityManager em => World.DefaultGameObjectInjectionWorld.EntityManager;

    static EntityArchetype archetype;
    static RenderMeshArray meshArray;
    static RenderMeshDescription meshDesc;
    static MaterialMeshInfo meshInfo;
    static bool initialized;

    public static void Initialize(Mesh mesh, Material mat, float scale)
    {
        if (initialized) return;
        initialized = true;

        // Render setup
        meshArray = new RenderMeshArray(new[]{ mat }, new[]{ mesh });
        meshDesc  = new RenderMeshDescription(
            shadowCastingMode: ShadowCastingMode.Off,
            receiveShadows:    false);
        meshInfo  = MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0);

        // Archetype
        archetype = em.CreateArchetype(
            typeof(LocalTransform),
            typeof(Velocity),
            typeof(Lifetime),
            typeof(RenderBounds),
            typeof(WorldRenderBounds),
            typeof(PerInstanceCullingTag),
            typeof(MaterialMeshInfo)
        );
    }

    public static Entity Spawn(float3 pos, float3 dir, float speed, float lifetime, float scale)
    {
        var e = em.CreateEntity(archetype);

        em.SetComponentData(e, new LocalTransform {
            Position = pos,
            Rotation = quaternion.identity,
            Scale    = scale
        });
        em.SetComponentData(e, new Velocity { Value = dir * speed });
        em.SetComponentData(e, new Lifetime { Remaining = lifetime });

        RenderMeshUtility.AddComponents(e, em, meshDesc, meshArray, meshInfo);

        return e;
    }
}
