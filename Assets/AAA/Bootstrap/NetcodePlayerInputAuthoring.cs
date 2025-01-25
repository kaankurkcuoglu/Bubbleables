using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

class NetcodePlayerInputAuthoring : MonoBehaviour
{
    class Baker : Baker<NetcodePlayerInputAuthoring>
    {
        public override void Bake(NetcodePlayerInputAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new NetcodePlayerInput());
        }
    }

}

public struct NetcodePlayerInput : IInputComponentData
{
    public float2 MovementInputVector;
    public InputEvent ShootInputEvent;
    public InputEvent RunInputEvent;
}

