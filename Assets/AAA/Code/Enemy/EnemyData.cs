using Unity.Entities;

namespace Game
{
    public struct EnemyData : IComponentData
    {
        public Entity TargetEntity;
        public float Speed;
    }
}