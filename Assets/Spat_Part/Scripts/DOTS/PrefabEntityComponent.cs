using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public class PrefabEntityComponent : IComponentData
{
    public Entity enemyPrefab;

    public Entity friendPrefab;

}


[MaterialProperty("_EnemyColor", MaterialPropertyFormat.Float4)]
public struct EnemyMaterialColor : IComponentData
{
    public float4 Value;
}