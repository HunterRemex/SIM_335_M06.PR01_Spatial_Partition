using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Info about this Enemy/Friendly instance
/// </summary>
public struct Soldier : IComponentData
{
    public int entityId;
    public int previousSoldier;
    public int nextSoldier;
    public float walkSpeed; //Should be prot'd
    public float3 position;
}