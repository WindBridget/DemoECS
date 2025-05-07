using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial class CleanupSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem _ecbSystem;
    const float Limit = 50f;

    protected override void OnCreate()
    {
        _ecbSystem = World.DefaultGameObjectInjectionWorld
            .GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
          .ForEach((Entity entity, int entityInQueryIndex, in LocalTransform lt) =>
          {
              if (math.abs(lt.Position.x) > Limit || math.abs(lt.Position.z) > Limit)
              {
                  ecb.DestroyEntity(entityInQueryIndex, entity);
              }
          })
          .ScheduleParallel();

        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}
