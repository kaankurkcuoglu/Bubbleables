using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

namespace Game
{
	[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
	public partial struct PlayerWeaponSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			return;
			var currentTime = (float)SystemAPI.Time.ElapsedTime;
			var ecb = new EntityCommandBuffer(Allocator.Temp);

			foreach (var (playerInput, playerWeapon, playerTransform, shootBuffer, ghostOwner) in SystemAPI
				         .Query<RefRO<PlayerInput>, RefRW<PlayerWeapon>, RefRO<LocalTransform>,
					         DynamicBuffer<ShootCommandBuffer>, RefRO<GhostOwner>>()
				         .WithAll<Simulate>())
			{
				switch (playerWeapon.ValueRW.State)
				{
					case FireState.NotRunning:
					{
						if (playerInput.ValueRO.IsFiring)
						{
							playerWeapon.ValueRW.State = FireState.Running;
							playerWeapon.ValueRW.LastFireTime = currentTime;
						}

						break;
					}
					case FireState.Running:
					{
						if (playerInput.ValueRO.IsFiring)
						{
							var timeSinceLastFire = currentTime - playerWeapon.ValueRW.LastFireTime;
							// 600 FireRate, 10 fire per second,  0.51f seconds passed -> 5 fire
							var firePerSecond = playerWeapon.ValueRW.FireRate / 60f;
							var timesFired = (int)math.floor(timeSinceLastFire * firePerSecond);
							playerWeapon.ValueRW.LastFireTime += timesFired / firePerSecond;

							for (int i = 0; i < timesFired; i++)
							{
								var fireDir = math.normalizesafe(playerInput.ValueRO.FirePos -
								                                 playerTransform.ValueRO.Position.xz);
								var projectileEntity = ecb.Instantiate(playerWeapon.ValueRW.ProjectilePrefab);
								var playerPosition = playerTransform.ValueRO.Position;
								const float forwardDist = 0.1f;

								ecb.SetComponent(projectileEntity, new LocalTransform
								{
									Position = playerPosition + fireDir.x0y() * forwardDist,
									Rotation = quaternion.identity,
									Scale = playerWeapon.ValueRW.ProjectileScale,
								});

								ecb.SetComponent(projectileEntity, ghostOwner.ValueRO);
								ecb.SetComponent(projectileEntity, new PhysicsVelocity
								{
									Linear = fireDir.x0y() * playerWeapon.ValueRW.ProjectileSpeed,
									Angular = float3.zero,
								});

								const float destroyDelay = 5f;
								ecb.SetComponent(projectileEntity, new Projectile
								{
									DestroyTime = currentTime + destroyDelay,
								});
							}
						}
						else
						{
							playerWeapon.ValueRW.State = FireState.NotRunning;
						}

						break;
					}
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			ecb.Playback(state.EntityManager);
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{
		}
	}
}