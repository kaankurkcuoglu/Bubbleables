using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
partial struct NetcodePlayerInputSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkStreamInGame>();
        state.RequireForUpdate<NetcodePlayerInput>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var netcodePlayerInput in SystemAPI.Query<RefRW<NetcodePlayerInput>>().WithAll<GhostOwnerIsLocal>())
        {
            float2 inputVector = new float2();

            if (Input.GetKey(KeyCode.W))
            {
                inputVector.y += 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                inputVector.y -= 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                inputVector.x -= 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                inputVector.x += 1;
            }
            
            netcodePlayerInput.ValueRW.InputVector = inputVector;
        }        
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
