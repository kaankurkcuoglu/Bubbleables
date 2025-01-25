using Game;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

namespace AAA.Bootstrap
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct FreezeRotationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (mass, _, entity) in SystemAPI.Query<RefRW<PhysicsMass>, RefRO<PlayerInput>>().WithEntityAccess())
            {
                mass.ValueRW.InverseInertia = 0;
                entityCommandBuffer.AddComponent<RotationLocked>(entity);
            }
            entityCommandBuffer.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }

    public struct RotationLocked : IComponentData
    {
    }
}