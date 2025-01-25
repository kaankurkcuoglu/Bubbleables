using UnityEngine;
using Unity.Entities;

namespace AAA.ElasticBalls.Springs.ECS
{
    public class BoneSphereAuthoring : MonoBehaviour
    {
        public float ColliderSize = 0.002f;
        public float RigidbodyMass = 1f;
        public float Spring = 100f;
        public float Damper = 0.2f;
        public ColliderShape ColliderShape = ColliderShape.Box;
        public Transform Armature;
    }
}