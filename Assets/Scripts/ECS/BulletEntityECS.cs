// File: BulletFactoryMiniECS.cs
using UnityEngine;
using MiniECS;
using MiniECS.Components;

/// <summary>
/// Factory tĩnh để sinh Bullet trong MiniECS-demo.
/// </summary>
public static class BulletEntityECS
{
    private static EntityManager _em;
    private static Mesh _mesh;
    private static Material _mat;
    private static bool _initialized = false;

    /// <summary>
    /// Khởi tạo Factory với EntityManager, Mesh và Material.
    /// Gọi 1 lần duy nhất trước khi Spawn.
    /// </summary>
    public static void Initialize(EntityManager em, Mesh mesh, Material mat)
    {
        _em = em;
        _mesh = mesh;
        _mat = mat;
        _initialized = true;
    }

    /// <summary>
    /// Sinh một bullet với vị trí, hướng, tốc độ và thời gian sống cho trước.
    /// </summary>
    public static int Spawn(Vector3 position, Vector3 direction, float speed, float lifetime)
    {
        if (!_initialized)
            throw new System.InvalidOperationException("BulletFactoryMiniECS chưa được Initialize!");

        // 1. Tạo entity
        var e = _em.CreateEntity();

        // 2. Gán Position & Velocity & Lifetime
        _em.AddComponent(e, new PositionECS { X = position.x, Y = position.y, Z = position.z });
        _em.AddComponent(e, new VelocityECS { VX = direction.x * speed, VY = direction.y * speed, VZ = direction.z * speed });
        _em.AddComponent(e, new LifetimeECS { Remaining = lifetime });

        // 3. Gán render data
        _em.AddComponent(e, new MeshECS { Mesh = _mesh });
        _em.AddComponent(e, new MaterialECS { Mat = _mat });

        return e;
    }
}
