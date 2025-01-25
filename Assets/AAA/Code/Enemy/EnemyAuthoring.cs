using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class EnemyAuthoring : MonoBehaviour
    {
        private class EnemyAuthoringBaker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                var config = Resources.Load<GameConfig>("GameConfig");
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent<EnemyTag>(entity);
                AddComponent(entity, new EnemyData
                {
                    Speed = config.EnemySpeed,
                    TargetEntity = Entity.Null
                });

                var gameConfig = Resources.Load<GameConfig>("GameConfig");

                // Health setup
                {
                    AddComponent(entity, new Alive());
                    AddComponent(entity, new Health { Value = gameConfig.InitialPlayerHealth });
                    AddBuffer<DamageBuffer>(entity);
                }
            }
        }
    }
}