using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
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

			foreach (var (enemySpawner, entity) in SystemAPI.Query<RefRW<EnemySpawner>>().WithEntityAccess())
			{
				const float spawnHeight = 1.5f;
				const float spawnRadius = 50f;
				var entities = state.EntityManager.Instantiate(enemySpawner.ValueRO.Enemy,
					enemySpawner.ValueRO.EnemyCount, Allocator.Temp);

				for (int i = 0; i < entities.Length; i++)
				{
					var spawnPos =
						enemySpawner.ValueRW.Random.NextFloat2(new float2(-spawnRadius), new float2(spawnRadius));

					ecb.SetComponent(entities[i], new LocalTransform
					{
						Position = new float3(spawnPos.x, spawnHeight, spawnPos.y),
						Rotation = quaternion.identity,
						Scale = 1f,
					});
				}

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