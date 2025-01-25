using System;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Random = Unity.Mathematics.Random;

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

		public static EntityManager GetEntityManager()
		{
			return GetClientWorld().EntityManager;
		}

		public static Entity GetLocalPlayerEntity()
		{
			var em = GetClientWorld().EntityManager;
			var query = new EntityQueryBuilder(Allocator.Temp)
				.WithAll<GhostOwnerIsLocal>()
				.Build(em);

			return query.ToEntityArray(Allocator.Temp)[0];
		}

		public static World GetClientWorld()
		{
			foreach (var world in World.All)
			{
				if (world.IsClient())
					return world;
			}

			throw new Exception("There's no Client World");
		}
	}
}