using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
public partial class LifetimeSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem _ecbSystem;

    protected override void OnCreate()
    {
        // Returns the real SystemBase instance
        _ecbSystem = World.DefaultGameObjectInjectionWorld
            .GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float dt = SystemAPI.Time.DeltaTime;
        // Tạo ECB writer để chạy song song
        var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
          .ForEach((Entity entity, int entityInQueryIndex, ref Lifetime life) =>
          {
              life.Remaining -= dt;
              if (life.Remaining <= 0f)
              {
                  // đẩy lệnh Destroy vào ECB
                  ecb.DestroyEntity(entityInQueryIndex, entity);
              }
          })
          .ScheduleParallel();

        // Đăng ký dependency để ECB system biết job còn chạy
        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}
