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

        public Dictionary<int, GameObject> bubbles = new Dictionary<int, GameObject>();
        
        protected override void OnCreate()
        {
            bubblePrefab = Object.FindFirstObjectByType<BubbleReference>().BubblePrefab;
        }


        protected override void OnUpdate()
        {
            foreach (var (netcodePlayer, transform, ghostOwner) in SystemAPI.Query<RefRO<NetcodePlayer>, RefRO<LocalTransform>, RefRO<GhostOwner>>())
            {
                if (!bubbles.TryGetValue(ghostOwner.ValueRO.NetworkId, out var bubble))
                {
                    bubble = Object.Instantiate(bubblePrefab);
                    bubbles.Add(ghostOwner.ValueRO.NetworkId, bubble);
                }
                
                bubble.transform.position = transform.ValueRO.Position;
            }
        }
    }
    
}