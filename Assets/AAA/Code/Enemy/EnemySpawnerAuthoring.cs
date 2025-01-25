using Unity.Entities;
using UnityEngine;

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
				});
			}
		}
	}
}