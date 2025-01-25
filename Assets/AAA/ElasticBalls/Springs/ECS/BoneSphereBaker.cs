using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace AAA.ElasticBalls.Springs.ECS
{

    public class BoneSphereBaker : Baker<BoneSphereAuthoring>
    {
        public override void Bake(BoneSphereAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
        
            AddComponent(entity, new BoneSettings
            {
                ColliderSize = authoring.ColliderSize,
                Mass = authoring.RigidbodyMass,
                Shape = authoring.ColliderShape
            });

            // Add physics components
            var massProperties = new PhysicsMass
            {
                Transform = new RigidTransform(quaternion.identity, float3.zero),
                InverseMass = math.rcp(authoring.RigidbodyMass)
            };
            AddComponent(entity, massProperties);

            // Add collider based on shape
            BlobAssetReference<Collider> collider;    
            if (authoring.ColliderShape == ColliderShape.Box)
            {
                collider = BoxCollider.Create(
                    new BoxGeometry
                    {
                        Center = float3.zero,
                        Size = new float3(authoring.ColliderSize),
                        Orientation = quaternion.identity
                    });
            }
            else
            {
                collider = SphereCollider.Create(
                    new SphereGeometry
                    {
                        Center = float3.zero,
                        Radius = authoring.ColliderSize
                    });
            }
            
            AddComponent(entity, new PhysicsCollider { Value = collider });
        }
    }
}