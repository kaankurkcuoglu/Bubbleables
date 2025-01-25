using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

class PlayerAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new NetcodePlayerInput());
            AddComponent(entity, new NetcodePlayer());
        }
    }

}

public struct NetcodePlayerInput : IInputComponentData
{
    public float2 MovementInputVector;
    public InputEvent ShootInputEvent;
    public InputEvent RunInputEvent;
}

public struct NetcodePlayer : IComponentData
{
}

