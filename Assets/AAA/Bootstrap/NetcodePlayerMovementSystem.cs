using Game;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
partial struct NetcodePlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;
        
        foreach (var (netcodePlayerInput, localTransform) in SystemAPI.Query<RefRO<PlayerInput>, RefRW<LocalTransform>>().WithAll<Simulate>())
        {
            float3 movementInput = new float3(netcodePlayerInput.ValueRO.MovementInputVector.x, 0, netcodePlayerInput.ValueRO.MovementInputVector.y);
            var runMultiplier = netcodePlayerInput.ValueRO.RunInputEvent.IsSet ? 2f : 1f;
            localTransform.ValueRW.Position +=  movementInput * dt * 10 * runMultiplier;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
