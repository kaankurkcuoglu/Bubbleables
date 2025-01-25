using Unity.Entities;
using Unity.NetCode;

namespace Game
{
	public struct Health : IComponentData
	{
		[GhostField]
		public int Value;
	}
}