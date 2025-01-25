using Unity.Mathematics;
using Unity.NetCode;

namespace Game
{
	public struct PlayerInput : IInputComponentData
	{
		public float2 MovementInputVector;
		public InputEvent ShootInputEvent;
		public InputEvent RunInputEvent;
		public bool IsFiring;
		public float2 FirePos;
	}
}