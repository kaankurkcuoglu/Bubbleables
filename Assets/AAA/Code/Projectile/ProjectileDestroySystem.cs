using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Game
{
	[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
	public partial struct ProjectileDestroySystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);
			var time = (float)SystemAPI.Time.ElapsedTime;

			foreach (var (projectile, entity) in SystemAPI.Query<RefRO<Projectile>>().WithEntityAccess())
			{
				if (time > projectile.ValueRO.DestroyTime)
				{
					ecb.DestroyEntity(entity);
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