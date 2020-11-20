using UnityEngine;
using System.Collections;
using System.Threading;
using Unity.Entities;
using Unity.Mathematics;

public struct Friendly : IComponentData
{

    public Soldier soldierData;



    //
    // public float3 targetClosestEnemyPos { get; set; }

    // public Friendly(int id)
    // {
    //     this.soldierData.id = id;
    //     this.soldierData.walkSpeed = 2f;
    //     this.soldierData.previousSoldier = -1;
    //     this.soldierData.nextSoldier = -1;
    // } //DOTS



    /// <summary>
    /// NOTE: Child of Soldier, Impl IComponentData. NO LOGIC
    /// </summary>

//     //Move to clsoest enemy within grid
//     public void Move(Soldier closestEnemy)
//     {
//         Debug.Log("Updated Friend");
//         //Rot to closest enemy
//         //soldierTrans.rotation = Quaternion.LookRotation(closestEnemy.soldierTrans.position - soldierTrans.position);

//         //soldierTrans.Translate(Vector3.forward * Time.fixedDeltaTime * soldierData.walkSpeed);
//     }
}
