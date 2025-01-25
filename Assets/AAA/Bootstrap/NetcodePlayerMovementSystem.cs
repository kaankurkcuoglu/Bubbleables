using Game;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
partial struct NetcodePlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // No initialization needed for now
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;

        // Get the PhysicsWorld singleton to perform raycasting
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach (var (netcodePlayerInput, localTransform, velocity) in SystemAPI.Query<RefRO<PlayerInput>, RefRW<LocalTransform>, RefRW<PhysicsVelocity>>().WithAll<Simulate>())
        {
            // Define the player's position and raycast parameters
            var playerPosition = localTransform.ValueRW.Position;
            float3 rayStart = playerPosition + new float3(0, 0f, 0);  // Start the ray slightly above the player
            float3 rayEnd = playerPosition + new float3(0, -1.85f, 0);   // End the ray below the player's feet

            
            
            // Set up the raycast input
            var raycastInput = new RaycastInput
            {
                Start = rayStart,
                End = rayEnd,
                Filter = new CollisionFilter
                {
                    BelongsTo =  ~0u, // Belongs to all layers
                    CollidesWith = 1u << 7, // Collides with layer 1
                    GroupIndex = 0
                }
            };

            // Perform the raycast
            bool isGrounded = true;//physicsWorld.CastRay(raycastInput, out var hit);

            // Debug visualization (optional, remove for production)
            Debug.DrawLine(rayStart, rayEnd, isGrounded ? Color.green : Color.red);

            // Apply movement
            float3 movementInput = new float3(netcodePlayerInput.ValueRO.MovementInputVector.x, 0, netcodePlayerInput.ValueRO.MovementInputVector.y);
            if (math.length(movementInput) > 0)
            {
                velocity.ValueRW.Linear += movementInput * 50 * SystemAPI.Time.DeltaTime; // Apply horizontal movement
                //Clamp the velocity to a maximum value
                if (math.length(velocity.ValueRO.Linear) > 10)
                {
                    var ySpeed = velocity.ValueRO.Linear.y;
                    velocity.ValueRW.Linear = math.normalize(velocity.ValueRO.Linear) * 10;
                    velocity.ValueRW.Linear.y = ySpeed;
                }
            }
            else
            {
                velocity.ValueRW.Linear.x = 0;
                velocity.ValueRW.Linear.z = 0;
            }
            
            // Apply jump only if the player is grounded
            if (isGrounded && netcodePlayerInput.ValueRO.JumpInputEvet.IsSet)
            {
                velocity.ValueRW.Linear.y = 7; // Apply upward velocity for jump
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        // No cleanup needed for now
    }
}
