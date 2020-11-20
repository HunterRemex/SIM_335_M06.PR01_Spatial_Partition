using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TargetPosition : IComponentData
{
    public float3 Value;
}
