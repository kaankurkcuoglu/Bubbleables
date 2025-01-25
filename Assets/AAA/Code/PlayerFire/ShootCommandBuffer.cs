using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Game
{
	public struct ShootCommandBuffer : IBufferElementData
	{
		[GhostField]
		public float2 Value;
	}
}