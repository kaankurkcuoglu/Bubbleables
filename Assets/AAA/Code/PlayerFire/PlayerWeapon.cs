using Unity.Entities;

namespace Game
{
	public struct PlayerWeapon : IComponentData
	{
		public int FireRate;
		public int ShootCommands;
		public float LastFireTime;
		public FireState State;
	}
}