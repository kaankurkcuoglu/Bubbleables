using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

namespace Game
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct EnemyFollowSystem : ISystem
    {
        private Random _random;

        public void OnCreate(ref SystemState state)
        {
            // Initialize the random number generator with a seed
            _random = new Random((uint)UnityEngine.Random.Range(1, int.MaxValue));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var localTransformLookup = state.GetComponentLookup<LocalTransform>(true);
            
            foreach (var (enemyData, physicsVelocity, localTransform) in 
                     SystemAPI.Query<RefRW<EnemyData>, RefRW<PhysicsVelocity>, RefRW<LocalTransform>>())
            {
                if (enemyData.ValueRW.TargetEntity == Entity.Null)
                {
                    enemyData.ValueRW.TargetEntity = ECSHelper.GetRandomPlayer(ref _random, ref state);
                }
                else
                {
                    if (localTransformLookup.TryGetComponent(enemyData.ValueRO.TargetEntity, out var targetTransform))
                    {
                        var targetPosition = targetTransform.Position;
                        var enemyPosition = localTransform.ValueRO.Position;
                        var direction = math.normalize(targetPosition - enemyPosition);
                        physicsVelocity.ValueRW.Linear = direction * enemyData.ValueRO.Speed;
                        localTransform.ValueRW.Rotation = quaternion.LookRotation(direction, math.up());
                    }
                }
                
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}