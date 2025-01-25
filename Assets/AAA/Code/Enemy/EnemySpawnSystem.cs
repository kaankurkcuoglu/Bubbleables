using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Game
{
	[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
	public partial struct EnemySpawnSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<EnemySpawner>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			Debug.Log("Run EnemySpawnSystem");
			var ecb = new EntityCommandBuffer(Allocator.Temp);

			foreach (var (enemySpawner, entity) in SystemAPI.Query<RefRO<EnemySpawner>>().WithEntityAccess())
			{
				state.EntityManager.Instantiate(enemySpawner.ValueRO.Enemy, enemySpawner.ValueRO.EnemyCount,
					Allocator.Temp);
				ecb.DestroyEntity(entity);
			}

			ecb.Playback(state.EntityManager);
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{
		}
	}
}