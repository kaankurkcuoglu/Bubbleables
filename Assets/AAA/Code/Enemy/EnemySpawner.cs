using Unity.Entities;

namespace Game
{
	public struct EnemySpawner : IComponentData
	{
		public int EnemyCount;
		public Entity Enemy;
	}
}