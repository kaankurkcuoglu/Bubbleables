using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
	public struct EnemySpawner : IComponentData
	{
		public int EnemyCount;
		public Entity Enemy;
		public Random Random;
	}
}