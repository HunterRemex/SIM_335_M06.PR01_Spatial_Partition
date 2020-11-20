using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct Soldier : IComponentData {
    // public int id;

    public int entityId;
    public float walkSpeed; //Should be prot'd
    public int previousSoldier;
    public int nextSoldier;

    public float3 position;

}