using Unity.Cinemachine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Game
{
	[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
	public partial class CameraFollowClientSystem : SystemBase
	{
		private CinemachineCamera _virtualCamera;
		private CameraTargetTransform _cameraTargetTransform;

		protected override void OnCreate()
		{
			// Find the Cinemachine Virtual Camera in the scene
			// _virtualCamera = Object.FindObjectOfType<CinemachineCamera>();
			// if (_virtualCamera == null)
			// {
			//     Debug.LogError("No Cinemachine Virtual Camera found in the scene!");
			// }
			//
			// _cameraTargetTransform = Object.FindObjectOfType<CameraTargetTransform>();
			// if (_cameraTargetTransform == null)
			// {
			//     Debug.LogError("No Camera Target Transform found in the scene!");
			// }
			//
			// _virtualCamera.Follow = _cameraTargetTransform.transform;
		}

		protected override void OnUpdate()
		{
			// if (_virtualCamera == null || _cameraTargetTransform == null)
			// {
			//     Debug.Log("Returning early because the virtual camera or camera target transform is null");
			//     return;
			// }

			// Query the player entity
			// foreach (var (localTransform, entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<GhostOwnerIsLocal>().WithEntityAccess())
			// {
			//     _cameraTargetTransform.transform.position = localTransform.ValueRO.Position;
			// }

			var localPlayerQuery = new EntityQueryBuilder(Allocator.Temp)
				.WithAll<GhostOwnerIsLocal>()
				.WithAll<LocalTransform>().Build(this);

			var localPlayerArray = localPlayerQuery.ToEntityArray(Allocator.Temp);
			if (localPlayerArray.Length == 0)
				return;

			var localTransform = EntityManager.GetComponentData<LocalTransform>(localPlayerArray[0]);
			Camera.main.transform.position = localTransform.Position + new float3(0f, 10f, -10f);
			Camera.main.transform.LookAt(localTransform.Position);
		}
	}
}