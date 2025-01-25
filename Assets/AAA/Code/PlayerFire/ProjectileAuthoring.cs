using Unity.Entities;
using UnityEngine;

namespace Game
{
	public class ProjectileAuthoring : MonoBehaviour
	{
		private class ProjectileAuthoringBaker : Baker<ProjectileAuthoring>
		{
			public override void Bake(ProjectileAuthoring authoring)
			{
				AddComponent(GetEntity(authoring, TransformUsageFlags.Dynamic), new Projectile());
			}
		}
	}
}