using System.Collections.Generic;
using Game;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace AAA.Bootstrap
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class BubbleFollowSystem : SystemBase
    {
        GameObject bubblePrefab;
        public Dictionary<int, (BoneSphere, GameObject)> bubbles = new Dictionary<int, (BoneSphere, GameObject)>();
        
        
        protected override void OnCreate()
        {
            bubblePrefab = Object.FindFirstObjectByType<BubbleReference>().BubblePrefab;
        }


        protected override void OnUpdate()
        {
            foreach (var (netcodePlayer, transform, ghostOwner) in SystemAPI.Query<RefRO<PlayerTag>, RefRO<LocalTransform>, RefRO<GhostOwner>>())
            {
                if (!bubbles.TryGetValue(ghostOwner.ValueRO.NetworkId, out var followers))
                {
                    var bubble = Object.Instantiate(bubblePrefab);
                    var playerModels = Resources.Load<GameConfig>("GameConfig").PlayerModels;
                    var playerModel = Object.Instantiate(playerModels[Random.Range(0, playerModels.Count)]);
                    followers = (bubble.GetComponent<BoneSphere>(), playerModel);
                    bubbles.Add(ghostOwner.ValueRO.NetworkId, followers);
                }
                
                
                followers.Item1.transform.position = transform.ValueRO.Position;
                followers.Item2.transform.position = transform.ValueRO.Position;
                // bubble.SetRootPosition(transform.ValueRO.Position);
            }
        }
    }
    
}