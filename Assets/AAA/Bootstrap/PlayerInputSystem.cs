using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Game
{
	[UpdateInGroup(typeof(GhostInputSystemGroup))]
	public partial class PlayerInputSystem : SystemBase
	{
		protected override void OnUpdate()
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

					var plane = new Plane(Vector3.up, Vector3.zero);
					var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					plane.Raycast(ray, out var enter);
					var hitPos = ray.GetPoint(enter);
					input.FirePos = ((float3)hitPos).xz;
				}

				netcodePlayerInput.ValueRW = input;

				if (Input.GetKeyDown(KeyCode.Space))
				{
					netcodePlayerInput.ValueRW.JumpInputEvet.Set();
				}
				else
				{
					netcodePlayerInput.ValueRW.JumpInputEvet = default;
				}
			}
		}
	}
}