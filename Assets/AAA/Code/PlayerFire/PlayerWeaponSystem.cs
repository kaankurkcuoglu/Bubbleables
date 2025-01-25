using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
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
			var currentTime = (float)SystemAPI.Time.ElapsedTime;

			foreach (var (playerInput, playerWeapon, playerTransform, shootBuffer) in SystemAPI
				         .Query<RefRO<PlayerInput>, RefRW<PlayerWeapon>, RefRO<LocalTransform>,
					         DynamicBuffer<ShootCommandBuffer>>()
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
								var fireDir = math.normalizesafe(playerInput.ValueRO.FirePos - playerTransform.ValueRO.Position.xz);
								shootBuffer.Add(new ShootCommandBuffer { Value = fireDir });
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
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{
		}
	}
}