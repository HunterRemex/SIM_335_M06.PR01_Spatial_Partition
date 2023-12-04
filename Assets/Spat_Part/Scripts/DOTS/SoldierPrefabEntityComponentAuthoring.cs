using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using Unity.Mathematics;

public struct SoldierPrefabEntityComponent : IComponentData
{
    public Entity EnemyPrefab;
    public Entity FriendPrefab;
}

public class SoldierPrefabEntityComponentAuthoring : MonoBehaviour
{
    [SerializeField]
    public GameObject enemyPrefab;
    [SerializeField]
    public GameObject friendPrefab;
}

public class SoldierPrefabEntityComponentComponentAuthoringBaker : Baker<SoldierPrefabEntityComponentAuthoring>
{
    public override void Bake(SoldierPrefabEntityComponentAuthoring authoring)
    {
        // Register prefab in baker
        Entity enemyPrefab = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic);
        Entity friendPrefab = GetEntity(authoring.friendPrefab, TransformUsageFlags.Dynamic);

        // Add Entity ref to a component for instantiation later?
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new SoldierPrefabEntityComponent
        {
            EnemyPrefab = enemyPrefab,
            FriendPrefab = friendPrefab,
        });
    }
}

// [MaterialProperty("_EnemyColor", MaterialPropertyFormat.Float4)]
public struct EnemyMaterialColor : IComponentData
{
    public float4 Value;
}