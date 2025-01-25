using Unity.Entities;

namespace Game
{
	public struct Projectile : IComponentData
	{
		public float DestroyTime;
	}
}