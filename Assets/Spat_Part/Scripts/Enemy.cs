using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct Enemy : IComponentData
{
    [SerializeField]
    public float3 oldPos { get; set; } ///Not getting set correctly
    
    //Pos soldier going towards
    public Soldier soldierData;
}