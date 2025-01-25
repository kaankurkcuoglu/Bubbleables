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