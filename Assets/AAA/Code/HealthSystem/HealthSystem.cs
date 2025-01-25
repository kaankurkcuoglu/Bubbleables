using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Game
{
	[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
	public partial struct HealthSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);

			foreach (var (health, damageBuffer, entity) in
			         SystemAPI.Query<RefRW<Health>, DynamicBuffer<DamageBuffer>>().WithEntityAccess())
			{
				for (int i = 0; i < damageBuffer.Length; i++)
				{
					health.ValueRW.Value -= damageBuffer[i].Damage;
				}

				if (health.ValueRW.Value <= 0f)
				{
					ecb.SetComponentEnabled<Alive>(entity, false);
				}


				damageBuffer.Clear();
			}
			
			ecb.Playback(state.EntityManager);
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{
		}
	}
}