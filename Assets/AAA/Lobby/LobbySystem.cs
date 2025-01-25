using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace AAA.Lobby
{
    
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct LobbySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkStreamConnection>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (connection,entity) in SystemAPI.Query<RefRO<NetworkStreamConnection>>().WithNone<ConnectedClient>().WithEntityAccess())
            {
                if (connection.ValueRO.CurrentState == ConnectionState.State.Connected)
                {
                    buffer.AddComponent(entity, new ConnectedClient());
                    Debug.Log("Client connected");

                    if (state.World.IsServer())
                    {
                        Debug.Log("Client connected to the server");
                    }
                }
            }
            buffer.Playback(state.EntityManager);
        }
    }
    
    public struct ConnectedClient : IComponentData
    {
    }
}