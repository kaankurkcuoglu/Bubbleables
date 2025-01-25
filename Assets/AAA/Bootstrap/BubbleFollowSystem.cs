using System.Collections.Generic;
using Game;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AAA.Bootstrap
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class BubbleFollowSystem : SystemBase
    {
        GameObject bubblePrefab;
        public Dictionary<int, (BoneSphere, PlayerAnimatorController)> playerFollowers = new Dictionary<int, (BoneSphere, PlayerAnimatorController)>();
        
        
        protected override void OnCreate()
        {
            bubblePrefab = Object.FindFirstObjectByType<BubbleReference>().BubblePrefab;
        }


        protected override void OnUpdate()
        {
            foreach (var (_, transform, ghostOwner, velocity) in SystemAPI.Query<RefRO<PlayerTag>, RefRO<LocalTransform>, RefRO<GhostOwner>, RefRO<PhysicsVelocity>>())
            {
                if (!playerFollowers.TryGetValue(ghostOwner.ValueRO.NetworkId, out var followers))
                {
                    var bubble = Object.Instantiate(bubblePrefab);
                    var playerModels = Resources.Load<GameConfig>("GameConfig").PlayerModels;
                    var playerModel = Object.Instantiate(playerModels[Random.Range(0, playerModels.Count)]);
                    followers = (bubble.GetComponent<BoneSphere>(), playerModel.GetComponent<PlayerAnimatorController>());
                    playerFollowers.Add(ghostOwner.ValueRO.NetworkId, followers);
                }
                
                
                followers.Item1.transform.position = transform.ValueRO.Position;
                followers.Item2.transform.position = transform.ValueRO.Position;
                
                var movementDelta = math.length(velocity.ValueRO.Linear);
                followers.Item2.SetSpeed(movementDelta);
                
                var lookdirection = velocity.ValueRO.Linear;
                if (math.length(lookdirection) > 0)
                {
                    followers.Item1.transform.rotation = quaternion.LookRotation(lookdirection, math.up());
                    followers.Item2.transform.rotation = quaternion.LookRotation(lookdirection, math.up());
                }
            }
        }
    }
    
}