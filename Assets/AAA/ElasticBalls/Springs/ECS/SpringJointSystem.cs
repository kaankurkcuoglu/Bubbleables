using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Transforms;

namespace AAA.ElasticBalls.Springs.ECS
{

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct SpringJointSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (spring, transform, physicsVelocity) 
            in SystemAPI.Query<RefRW<SpringJointData>, RefRO<LocalTransform>, RefRW<PhysicsVelocity>>())
        {
            var connectedTransform = SystemAPI.GetComponent<LocalTransform>(spring.ValueRO.ConnectedEntity);
            
            // Calculate spring force
            var offset = transform.ValueRO.Position - connectedTransform.Position;
            var velocity = physicsVelocity.ValueRO.Linear;
            
            var force = -spring.ValueRO.Spring * offset 
                       - spring.ValueRO.Damper * velocity;
            
            // Apply force
            physicsVelocity.ValueRW.Linear += force * deltaTime;
        }
    }
}
}