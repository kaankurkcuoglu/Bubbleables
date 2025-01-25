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
        foreach (var (netcodePlayerInput, localTransform) in SystemAPI.Query<RefRO<NetcodePlayerInput>, RefRW<LocalTransform>>().WithAll<Simulate>())
        {
            float3 asd = new float3(netcodePlayerInput.ValueRO.InputVector.x, 0, netcodePlayerInput.ValueRO.InputVector.y);
            localTransform.ValueRW.Position +=  asd * Time.deltaTime * 10;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
