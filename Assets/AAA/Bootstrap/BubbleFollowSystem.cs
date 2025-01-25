using System.Collections.Generic;
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

        public Dictionary<int, BoneSphere> bubbles = new Dictionary<int, BoneSphere>();
        
        protected override void OnCreate()
        {
            bubblePrefab = Object.FindFirstObjectByType<BubbleReference>().BubblePrefab;
        }


        protected override void OnUpdate()
        {
            foreach (var (netcodePlayer, transform, ghostOwner) in SystemAPI.Query<RefRO<PlayerTag>, RefRO<LocalTransform>, RefRO<GhostOwner>>())
            {
                if (!bubbles.TryGetValue(ghostOwner.ValueRO.NetworkId, out var bubble))
                {
                    bubble = Object.Instantiate(bubblePrefab).GetComponent<BoneSphere>();
                    bubbles.Add(ghostOwner.ValueRO.NetworkId, bubble);
                }
                
                bubble.transform.position = transform.ValueRO.Position;
                // bubble.SetRootPosition(transform.ValueRO.Position);
            }
        }
    }
    
}