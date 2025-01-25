using System.Diagnostics;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Game
{
	public class EnemySpawnerAuthoring : MonoBehaviour
	{
		private class EnemySpawnerAuthoringBaker : Baker<EnemySpawnerAuthoring>
		{
			public override void Bake(EnemySpawnerAuthoring authoring)
			{
				var config = Resources.Load<GameConfig>("GameConfig");
				var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
				AddComponent(entity, new EnemySpawner
				{
					Enemy = GetEntity(config.EnemyPrefab),
					EnemyCount = config.EnemyCount,
					Random = new Random((uint)Stopwatch.GetTimestamp()),
				});
			}
		}
	}
}