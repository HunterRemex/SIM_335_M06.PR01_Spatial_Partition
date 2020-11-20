using UnityEngine;
using System.Collections;
using System;
using System.ComponentModel;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public struct Enemy : IComponentData
{
    [SerializeField]
    public float3 oldPos { get; set; } ///Not getting set correctly
    
    
    //Pos soldier going towards
    public Soldier soldierData;





    //public Grid grid;

    //public EnemyData enemyData;


    // public Enemy(int id)
    // {
    //     targetPos = new float3(0, 0, 0);
    //     soldierData.id = id;
    //     //this.soldierData.walkSpeed = 5f;
    //     //this.enemyData.mapWidth = 20f;
    // }
    // public Enemy(GameObject soldierGO, float mapWidth, Grid grid)
    // {
    //     // //this.soldierTrans = soldierGO.transform;

    //     // this.soldierMeshRenderer = soldierGO.GetComponent<MeshRenderer>();

    //     // this.enemyData.mapWidth = mapWidth;

    //     // this.grid = grid;


    //     // //going live
    //     // grid.Add(this);

    //     // enemyData.oldPos = soldierTrans.position;

    //     // this.soldierData.walkSpeed = 5f;

    //     // GetNewTarget();

    // }



    // /// <summary>
    // /// NOTE: Child of Soldier, Impl IComponentData. NO LOGIC
    // /// </summary>
    // public void Move(Translation tx)
    // {
    //     // //Move to target
    //     // //soldierTrans.Translate(Vector3.forward * Time.fixedDeltaTime * soldierData.walkSpeed);

    //     // grid.Move(this, enemyData.oldPos);

    //     // enemyData.oldPos = soldierTrans.position;

    //     // if ((soldierTrans.position - enemyData.currentTarget).magnitude < 1f)
    //     // {
    //     //     GetNewTarget();
    //     // }
    //     // Debug.Log(tx); //Runs, called from SoldierSpawner

    // }

    // void GetNewTarget()
    // {
    //     //enemyData.currentTarget = new Vector3(UnityEngine.Random.Range(0f, enemyData.mapWidth), 0.5f, UnityEngine.Random.Range(0f, enemyData.mapWidth));

    //     //soldierTrans.rotation = Quaternion.LookRotation(enemyData.currentTarget - soldierTrans.position);
    // }
}
