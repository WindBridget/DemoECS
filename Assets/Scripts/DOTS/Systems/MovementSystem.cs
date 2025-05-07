using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public partial class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float dt = SystemAPI.Time.DeltaTime;
        Entities
          .ForEach((ref LocalTransform lt, in Velocity v) =>
          {
              lt.Position += v.Value * dt;
          })
          .ScheduleParallel();
    }
}
