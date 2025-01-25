using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

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
            using var drvQuery = state.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());

            foreach (var (connection, entity) in SystemAPI.Query<RefRO<NetworkStreamConnection>>()
                         .WithNone<ConnectedClient>().WithEntityAccess())
            {
                if (connection.ValueRO.CurrentState == ConnectionState.State.Connected)
                {
                    var driver = drvQuery.GetSingletonRW<NetworkStreamDriver>();
                    var networkId = SystemAPI.GetComponentRO<NetworkId>(entity);

                    var connectedClient = new ConnectedClient()
                    {
                        ConnectionId = networkId.ValueRO.Value,
                        IpAddress = driver.ValueRO.GetRemoteEndPoint(connection.ValueRO).ToString()
                    };

                    buffer.AddComponent(entity, connectedClient);
                    buffer.AddComponent(entity, new NetworkStreamInGame());

                    Debug.Log("Client connected");

                    if (state.World.IsServer())
                    {
                        Debug.Log(
                            $"Client connected to the server {connectedClient.ConnectionId} {connectedClient.IpAddress}");
                    }
                }
            }
            
            buffer.Playback(state.EntityManager);
        }
    }

    public struct ConnectedClient : IComponentData
    {
        public int ConnectionId;
        public FixedString32Bytes IpAddress;
    }
}