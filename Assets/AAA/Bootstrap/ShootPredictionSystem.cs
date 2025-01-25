using Game;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
partial struct ShootPredictionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var input in SystemAPI.Query<RefRO<PlayerInput>>())
        {
            if (input.ValueRO.ShootInputEvent.IsSet)
            {
                
            }
            else
            {
                
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
