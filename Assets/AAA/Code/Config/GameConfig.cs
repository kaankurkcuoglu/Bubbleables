using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	[CreateAssetMenu]
	public class GameConfig : ScriptableObject
	{
		public float EnemyAttackRange = 2;
		public int EnemyAttackDamage = 1;
		public int InitialPlayerHealth = 100;
		public int InitialEnemyHealth = 100;
		public int EnemyCount = 100_000;
		public float EnemySpeed = 5f; 
		public int FireRate = 600;
		public GameObject EnemyPrefab;
		public List<GameObject> PlayerModels;
		public GameObject ProjectilePrefab;
		public float ProjectileSpeed;
		public float ProjectileScale;
	}
}