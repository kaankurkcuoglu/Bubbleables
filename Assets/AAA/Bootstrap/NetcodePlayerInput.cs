using Unity.Mathematics;
using Unity.NetCode;

public struct NetcodePlayerInput : IInputComponentData
{
	public float2 MovementInputVector;
	public InputEvent ShootInputEvent;
	public InputEvent RunInputEvent;
}