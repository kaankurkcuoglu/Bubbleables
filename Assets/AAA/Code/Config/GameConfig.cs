using UnityEngine;

namespace Game
{
	[CreateAssetMenu]
	public class GameConfig : ScriptableObject
	{
		public int InitialPlayerHealth = 100;
		public int InitialEnemyHealth = 100;
		public int EnemyCount = 100_000;
		public GameObject EnemyPrefab;
	}
}