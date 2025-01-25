using Unity.Entities;
using UnityEngine;


public class EntitiesReferencesAuthoring : MonoBehaviour
{
    public GameObject PlayerPrefabGameObject;
    public class Baker : Baker<EntitiesReferencesAuthoring>
    {
        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences
            {
                PlayerPrefabEntity = GetEntity(authoring.PlayerPrefabGameObject, TransformUsageFlags.Dynamic),
            });
        }
    }
}

public struct EntitiesReferences : IComponentData
{
    public Entity PlayerPrefabEntity;
}