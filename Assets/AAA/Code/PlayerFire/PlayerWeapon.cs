using Unity.Entities;
using Unity.NetCode;

namespace Game
{
	public struct PlayerWeapon : IComponentData
	{
		public int FireRate;
		public int ShootCommands;

		[GhostField]
		public Entity ProjectilePrefab;
		
		[GhostField]
		public float LastFireTime;

		[GhostField]
		public FireState State;
	}
}