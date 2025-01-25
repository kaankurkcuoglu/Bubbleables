using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace AAA.ElasticBalls.Springs.ECS
{
    public struct BoneTag : IComponentData
    {
    }

    public struct SpringJointData : IComponentData
    {
        public Entity ConnectedEntity;
        public float Spring;
        public float Damper;
    }

    public struct BoneSettings : IComponentData
    {
        public float ColliderSize;
        public float Mass;
        public ColliderShape Shape;
    }

    public enum ColliderShape
    {
        Box,
        Sphere
    }
}