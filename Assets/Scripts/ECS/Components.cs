using MiniECS;
using UnityEngine;

namespace MiniECS.Components
{
    public struct PositionECS : IComponent { public float X, Y, Z; }
    public struct VelocityECS : IComponent { public float VX, VY, VZ; }
    public struct LifetimeECS : IComponent { public float Remaining; }
    public struct BulletTagECS : IComponent { }
    public struct MeshECS : IComponent { public Mesh Mesh; }
    public struct MaterialECS : IComponent { public Material Mat; }
}
