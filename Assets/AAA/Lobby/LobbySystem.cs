using System.Linq;
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
                        IpAddress = driver.ValueRO.GetRemoteEndPoint(connection.ValueRO).Address
                    };

                    buffer.AddComponent(entity, connectedClient);
                    buffer.AddComponent(entity, new NetworkStreamInGame());

                    Debug.Log("Client connected");

                    if (state.World.IsServer())
                    {
                        Debug.Log(
                            $"Client connected to the server {connectedClient.ConnectionId} {connectedClient.IpAddress}");
                        
                        var mainRpcEntity = buffer.CreateEntity();
                        buffer.AddComponent(mainRpcEntity, new ClientConnectedRpc()
                        {
                            ConnectedClient = connectedClient
                        });
                        buffer.AddComponent(mainRpcEntity, new SendRpcCommandRequest());
                        
                        using var query = state.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<ConnectedClient>());
                        var connectedClients = query.ToEntityArray(Allocator.Temp);
                        
                        foreach (var client in connectedClients)
                        {
                            var connected = state.EntityManager.GetComponentData<ConnectedClient>(client);
                            
                            if (connected.ConnectionId == connectedClient.ConnectionId)
                            {
                                continue;
                            }
                            
                            var rpcEntity = buffer.CreateEntity();
                            buffer.AddComponent(rpcEntity, new ClientConnectedRpc()
                            {
                                ConnectedClient = connected
                            });
                            buffer.AddComponent(rpcEntity, new SendRpcCommandRequest());
                        }
                    }
                }
            }
            
            buffer.Playback(state.EntityManager);

            var clientBuffer = new EntityCommandBuffer(Allocator.Temp);
            if (!state.World.IsServer())
            {
                foreach (var (receiveRpcCommandRequest, entity) in SystemAPI
                             .Query<RefRO<ReceiveRpcCommandRequest>>()
                             .WithAll<ClientConnectedRpc>()
                             .WithEntityAccess())
                {
                    using var query = state.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<ConnectedClient>());
                    var connectedClients = query.ToEntityArray(Allocator.Temp);
                    
                    var connectedClient = SystemAPI.GetComponentRO<ClientConnectedRpc>(entity).ValueRO.ConnectedClient;
                    
                    var found = false;
                    foreach (var client in connectedClients)
                    {
                       var connected = state.EntityManager.GetComponentData<ConnectedClient>(client);
                       
                          if (connected.ConnectionId == connectedClient.ConnectionId)
                          {
                            found = true;
                            break;
                          }
                    }

                    if (!found)
                    {
                        var newConnectedClient = clientBuffer.CreateEntity();
                        
                        clientBuffer.AddComponent(newConnectedClient, connectedClient);
                    }
                    
                    clientBuffer.DestroyEntity(entity);
                }
            }
            clientBuffer.Playback(state.EntityManager);
        }
    }

    public struct ConnectedClient : IComponentData
    {
        public int ConnectionId;
        public FixedString32Bytes IpAddress;
    }
    
    public struct ClientConnectedRpc : IRpcCommand
    {
        public ConnectedClient ConnectedClient;
    }
}