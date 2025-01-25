using Game;
using Unity.Entities;
using UnityEngine;

class PlayerAuthoring : MonoBehaviour
{
	class Baker : Baker<PlayerAuthoring>
	{
		public override void Bake(PlayerAuthoring authoring)
		{
			var playerEntity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(playerEntity, new PlayerInput());
			AddComponent(playerEntity, new PlayerTag());

			var gameConfig = Resources.Load<GameConfig>("GameConfig");

			// Health setup
			{
				AddComponent(playerEntity, new Alive());
				AddComponent(playerEntity, new Health { Value = gameConfig.InitialPlayerHealth });
				AddBuffer<DamageBuffer>(playerEntity);
			}

			// Weapon setup
			{
				AddComponent(playerEntity, new PlayerWeapon
				{
					FireRate = gameConfig.FireRate,
					LastFireTime = 0f,
					State = FireState.NotRunning,
					ProjectilePrefab = GetEntity(gameConfig.ProjectilePrefab, TransformUsageFlags.Dynamic),
					ProjectileSpeed = gameConfig.ProjectileSpeed,
					ProjectileScale = gameConfig.ProjectileScale
				});

				AddBuffer<ShootCommandBuffer>(playerEntity);
			}
		}
	}
}