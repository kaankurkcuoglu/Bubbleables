using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Game
{
	[UpdateAfter(typeof(PlayerWeaponSystem))]
	[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
	public partial struct PlayerFireSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);
			
			foreach (var (playerWeapon, playerTransform, ghostOwner) in SystemAPI.Query<RefRW<PlayerWeapon>, RefRO<LocalTransform>, RefRO<GhostOwner>>())
			{
				while (playerWeapon.ValueRW.ShootCommands > 0)
				{
					playerWeapon.ValueRW.ShootCommands--;
					var projectileEntity = ecb.Instantiate(playerWeapon.ValueRW.ProjectilePrefab);
					var playerForward = playerTransform.ValueRO.Forward();
					var playerPosition = playerTransform.ValueRO.Position;
					const float forwardDist = 1.0f;
					var spawnPos = playerPosition + playerForward * forwardDist;

					ecb.SetComponent(projectileEntity, new LocalTransform
					{
						Position = spawnPos,
						Rotation = quaternion.identity,
						Scale = 1.0f,
					});
					
					ecb.SetComponent(projectileEntity, ghostOwner.ValueRO);
				}
			}
			
			ecb.Playback(state.EntityManager);
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{
		}
	}
}