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
    [SerializeField] float Speed = 5f;
    [SerializeField] float Lifetime = 10f;

    void Start()
    {
        BulletEntity.Initialize(BulletMesh, BulletMaterial, 0.1f);
    }


    public void Spawn() {
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
            BulletEntity.Spawn(pos, dir, Speed, Lifetime, 0.1f);
        }
    }

}
