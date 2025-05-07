using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public class DOTSBootstrap : MonoBehaviour
{
    [Header("Mesh & Material")]
    [SerializeField] Mesh BulletMesh;
    [SerializeField] Material BulletMaterial;

    [Header("Spawn Settings")]
    [SerializeField] int SpawnCount = 5000;
    [SerializeField] float Spread = 20f;
    [SerializeField] float Lifetime = 10f;

    EntityManager em;
    EntityArchetype archetype;
    RenderMeshArray meshArray;
    RenderMeshDescription meshDesc;
    MaterialMeshInfo meshInfo;

    void OnEnable()
    {
        // 1. Lấy EntityManager
        em = World.DefaultGameObjectInjectionWorld.EntityManager;

        // 2. Tạo RenderMeshArray chứa Mesh + Material
        meshArray = new RenderMeshArray(
            new Material[] { BulletMaterial },
            new Mesh[] { BulletMesh }
        );

        // 3. Định nghĩa RenderMeshDescription (bật/tắt shadow, culling…)
        meshDesc = new RenderMeshDescription(
            shadowCastingMode: ShadowCastingMode.Off,
            receiveShadows: false
        );

        // 4. Chọn chỉ số 0 trong RenderMeshArray (vì chỉ có 1 mesh + material)
        meshInfo = MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0);

        // 5. Tạo Archetype cho Bullet
        archetype = em.CreateArchetype(
            typeof(LocalTransform),   // bao gồm Position, Rotation, Scale
            typeof(Velocity),         // custom IComponentData
            typeof(Lifetime),         // custom IComponentData
                                      // Không thêm RenderMesh nữa!
            typeof(RenderBounds),
            typeof(WorldRenderBounds),
            typeof(PerInstanceCullingTag),
            typeof(MaterialMeshInfo)
        );

        // 6. Spawn entities
        for (int i = 0; i < SpawnCount; i++)
        {
            float3 pos = new float3(
                UnityEngine.Random.Range(-Spread, Spread),
                0f,
                UnityEngine.Random.Range(-Spread, Spread)
            );
            float3 dir = math.normalize(new float3(
                UnityEngine.Random.Range(-1f, 1f),
                0f,
                UnityEngine.Random.Range(-1f, 1f)
            ));

            var e = em.CreateEntity(archetype);

            // 6.1 Gán transform với scale 0.1f
            em.SetComponentData(e, new LocalTransform
            {
                Position = pos,
                Rotation = quaternion.identity,
                Scale = 0.1f
            });

            // 6.2 Gán logic chuyển động & thời gian sống
            em.SetComponentData(e, new Velocity { Value = dir * 5f });
            em.SetComponentData(e, new Lifetime { Remaining = Lifetime });

            // 6.3 Thêm component rendering (RenderMeshArray) — 
            //     đừng gọi overload dùng RenderMesh, mà dùng RenderMeshArray
            RenderMeshUtility.AddComponents(
                e,
                em,
                meshDesc,
                meshArray,
                meshInfo
            );
        }
    }

}
