using MiniECS;
using UnityEngine;

namespace MiniECS.Components
{
    public struct PositionECS : IComponent { public float X, Y; }
    public struct VelocityECS : IComponent { public float VX, VY; }
    public struct LifetimeECS : IComponent { public float Remaining; }
    public struct BulletTagECS : IComponent { }
    public struct MeshECS : IComponent { public Mesh Mesh; }
    public struct MaterialECS : IComponent { public Material Mat; }
}
