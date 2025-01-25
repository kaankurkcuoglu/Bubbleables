using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Game
{
	[UpdateInGroup(typeof(GhostInputSystemGroup))]
	partial struct NetcodePlayerInputSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<NetworkStreamInGame>();
			state.RequireForUpdate<PlayerInput>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var input = new PlayerInput();

			foreach (var netcodePlayerInput in SystemAPI.Query<RefRW<PlayerInput>>().WithAll<GhostOwnerIsLocal>())
			{
				float2 inputVector = new float2();

				if (Input.GetKey(KeyCode.W))
				{
					inputVector.y += 1;
				}

				if (Input.GetKey(KeyCode.S))
				{
					inputVector.y -= 1;
				}

				if (Input.GetKey(KeyCode.A))
				{
					inputVector.x -= 1;
				}

				if (Input.GetKey(KeyCode.D))
				{
					inputVector.x += 1;
				}

				if (Input.GetKey(KeyCode.LeftShift))
				{
					input.RunInputEvent.Set();
				}

				input.MovementInputVector = inputVector;

				if (Input.GetMouseButtonDown(0))
				{
					input.ShootInputEvent.Set();
				}

				if (Input.GetMouseButton(0))
				{
					input.IsFiring = true;
				}

				netcodePlayerInput.ValueRW = input;
			}
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{
		}
	}
}