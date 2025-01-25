using Unity.Entities;

namespace Game
{
    public struct EnemyData : IComponentData
    {
        public int AttackDamage;
        public float AttackRange;
        public Entity TargetEntity;
        public float Speed;
    }
}