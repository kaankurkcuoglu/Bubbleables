using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
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
			var currentTime = (float)SystemAPI.Time.ElapsedTime;
			
			foreach (var (playerWeapon, playerTransform, ghostOwner, shootCommands) in SystemAPI.Query<RefRW<PlayerWeapon>, RefRO<LocalTransform>, RefRO<GhostOwner>, DynamicBuffer<ShootCommandBuffer>>())
			{
				for (int i = 0; i < shootCommands.Length; i++)
				{
	
				}

				shootCommands.Clear();
			}
			
			ecb.Playback(state.EntityManager);
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{
		}
	}
}