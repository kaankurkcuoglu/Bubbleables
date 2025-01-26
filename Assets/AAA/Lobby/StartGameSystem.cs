using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using Random = UnityEngine.Random;


namespace AAA.Lobby
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct StartGameSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntitiesReferences>();
            state.RequireForUpdate<GameStartedTag>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
            var buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (networkId, entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<NetworkStreamInGame>().WithEntityAccess())
            {
                var playerEntity = buffer.Instantiate(entitiesReferences.PlayerPrefabEntity);
                buffer.SetComponent(playerEntity, LocalTransform.FromPosition(new float3(Random.Range(45,55),14f, Random.Range(-65,-60))));
                buffer.AddComponent(playerEntity, new GhostOwner
                {
                    NetworkId = networkId.ValueRO.Value
                });
                
                buffer.AddComponent(entity, new NetworkStreamInGame());
            }
            
            buffer.Playback(state.EntityManager);
        }
    }
    
    public struct GameStartedTag : IComponentData
    {
        
    }
}