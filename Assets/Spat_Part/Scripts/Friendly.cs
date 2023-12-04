using UnityEngine;
using System.Collections;
using System.Threading;
using Unity.Entities;
using Unity.Mathematics;

public struct Friendly : IComponentData
{
    public Soldier soldierData;
}