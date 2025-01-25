using Unity.Burst;
using Unity.Entities;

namespace Game
{
	[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
	public partial struct HealthSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{

			
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}
	}
}