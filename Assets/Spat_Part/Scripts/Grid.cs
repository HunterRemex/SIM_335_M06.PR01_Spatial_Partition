using System;
using System.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public struct GridData {
    public int cellSize;

    public Soldier[, , ] cells;
}

public class Grid {
    public Vector3 yes;
    public static Grid instance;

    public GridData data;

    public int numberOfCells;

    public Grid (int mapWidth, int cellSize) {
        data.cellSize = cellSize;

        int _numberOfCells = mapWidth / cellSize;

        numberOfCells = _numberOfCells;
        data.cells = new Soldier[numberOfCells, numberOfCells, numberOfCells];

        instance = this;
        // Debug.Log("Local Grid singled");
    }

    public int Add (Soldier soldier, float3 pos) {

        int cellX = Mathf.Min ((int) (pos.x / instance.data.cellSize), numberOfCells - 1);
        int cellY = Mathf.Min ((int) (pos.y / instance.data.cellSize), numberOfCells - 1);
        int cellZ = Mathf.Min ((int) (pos.z / instance.data.cellSize), numberOfCells - 1);

        //soldier.prevSoldier = null; ///HANDLED: after returns from this fn
        /// 
        soldier.nextSoldier = instance.data.cells[cellX, cellY, cellZ].entityId;

        int _oldLeaderSoldier = instance.data.cells[cellX, cellY, cellZ].entityId;
        instance.data.cells[cellX, cellY, cellZ] = soldier;

        // Debug.LogFormat (" incoming of {0} noting oldLeader {1} with next {2}", soldier.entityId, _oldLeaderSoldier, soldier.nextSoldier);

        //Debug.Log("Soldier in " + cellX + " " + cellZ + " with next being " + cells[cellX, cellZ]);
        //Debug.Log("Soldier added ID'd as " + soldier.id);

        if (soldier.nextSoldier > 0) {
            //soldier.nextSoldier.previousSoldier = soldier;
            //EntityManager.GetChunkComponentData<Enemy>()

            return soldier.nextSoldier;
        }
        return -1; //_oldLeaderSoldier; //Note who is ahead of us
    }

    // public int FindClosestEnemy(Soldier friendlySoldier, float3 pos)
    // {
    //     //Determine which grid cell the friendly soldier is in
    //     int cellX = (int)(pos.x / data.cellSize);
    //     int cellZ = (int)(pos.z / data.cellSize);

    //     //Get the first enemy in grid
    //     Soldier enemy = data.cells[cellX, cellZ];

    //     //Find the closest soldier of all in the linked list
    //     Soldier closestSoldier;

    //     float bestDistSqr = Mathf.Infinity;

    //     //Loop through the linked list
    //     while (enemy.id != 0) //Covers most cases
    //     {
    //         //The distance sqr between the soldier and this enemy
    //         Vector3 _enPos = enemy.position;
    //           Vector3 _frPos = friendlySoldier.position;
    //   float distSqr = (_enPos - _frPos).sqrMagnitude;

    //         //If this distance is better than the previous best distance, then we have found an enemy that's closer
    //         if (distSqr < bestDistSqr)
    //         {
    //             bestDistSqr = distSqr;

    //             closestSoldier = enemy;
    //         }

    //         //Get the next enemy in the list
    //         enemy = enemy.nextSoldier;
    //     }

    //     return closestSoldier;
    // }

    //A soldier in the grid has moved, so see if we need to update in which grid the soldier is
    public void Move (Soldier soldier, Vector3 oldPos) {
        // //See which cell it was in 
        // int oldCellX = (int)(oldPos.x / cellSize);
        // int oldCellZ = (int)(oldPos.z / cellSize);

        // //See which cell it is in now
        // int cellX = (int)(soldier.soldierTrans.position.x / cellSize);
        // int cellZ = (int)(soldier.soldierTrans.position.z / cellSize);

        // //If it didn't change cell, we are done
        // if (oldCellX == cellX && oldCellZ == cellZ)
        // {
        //     return;
        // }

        // //Unlink it from the list of its old cell
        // if (soldier.previousSoldier != null)
        // {
        //     soldier.previousSoldier.nextSoldier = soldier.nextSoldier;
        // }

        // if (soldier.nextSoldier != null)
        // {
        //     soldier.nextSoldier.previousSoldier = soldier.previousSoldier;
        // }

        // //If it's the head of a list, remove it
        // if (cells[oldCellX, oldCellZ] == soldier)
        // {
        //     cells[oldCellX, oldCellZ] = soldier.nextSoldier;
        // }

        // //Add it bacl to the grid at its new cell
        // Add(soldier);
    }

    public bool Equals (Grid other) {
        throw new NotImplementedException ();
    }

    // protected override void OnStartRunning()
    // {
    //     Debug.Log("Gridder ran");

    // }
    // protected override void OnUpdate()
    // {
    //     Debug.Log("Gridder ran up");

    // }

    // public int GetHashCode()
    // {
    //     return 0;
    // }
}