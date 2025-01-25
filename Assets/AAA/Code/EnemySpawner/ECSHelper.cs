using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Game
{
	public static class ECSHelper
	{
		public static Entity GetRandomPlayer(ref Random random, ref SystemState state)
		{
			var query = new EntityQueryBuilder(Allocator.Temp)
				.WithAll<PlayerTag>()
				.WithAll<GhostOwner>()
				.Build(ref state);
			var entities = query.ToEntityArray(Allocator.Temp);
			var entityIdx = random.NextInt(0, entities.Length);
			return entities[entityIdx];
		}
	}
}