using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject friendlyObj;
    public GameObject enemyObj;

    //Change mat  to det closest enemy
    public Material enemyMaterial;
    public Material closestEnemyMat;

    public Transform enemyParent;
    public Transform friendlyParent;

    List<Soldier> enemySoldiers = new List<Soldier>();
    List<Soldier> friendlySoldiers = new List<Soldier>();

    //Save closest enemy
    List<Soldier> closestEnemies = new List<Soldier>();

    //Grid Data
    float mapWidth = 50f;
    int cellSize = 10;


    //Num soldiers per team
    int numSoldiersPerTeam = 100;

    //The grid
    Grid grid;

    private void Start()
    {
        // grid = new Grid((int)mapWidth, cellSize);

        // for (int soldier = 0; soldier < numSoldiersPerTeam; soldier++)
        // {
        //     //Give enemies random pos's\
        //     Vector3 randomPos = new Vector3(Random.Range(0f, mapWidth), 0.5f, Random.Range(0f, mapWidth));

        //     GameObject newEnemy = Instantiate(enemyObj, randomPos, Quaternion.identity) as GameObject;

        //     enemySoldiers.Add(new Enemy(newEnemy, mapWidth, grid));

        //     //Parent in insp
        //     newEnemy.transform.parent = enemyParent;


        //     ///Spawn friendly
        //     randomPos = new Vector3(Random.Range(0f, mapWidth), 0.5f, Random.Range(0f, mapWidth));

        //     GameObject newFriend = Instantiate(friendlyObj, randomPos, Quaternion.identity) as GameObject;

        //     friendlySoldiers.Add(new Friendly(newFriend, mapWidth));

        //     newFriend.transform.parent = friendlyParent;
        // }
    }

    private void Update()
    {
        // //Move enemies
        // foreach (Enemy soldier in enemySoldiers)
        // {
        //     soldier.Move();
        // }

        // //Reset mat of closest enemy
        // foreach (Enemy soldier in closestEnemies)
        // {
        //     soldier.soldierMeshRenderer.material = enemyMaterial;
        // }

        // //Reset closest enemies list
        // closestEnemies.Clear();

        // //F/E friendly find closest enemy, change its color & chase
        // for (int friend = 0; friend < friendlySoldiers.Count; friend++)
        // {

        //     //Spat-Part
        //     Soldier closestEnemy = grid.FindClosestEnemy(friendlySoldiers[friend]);

        //     //If found enemy
        //     if (closestEnemy != null)
        //     {
        //         //Change mat
        //         closestEnemy.soldierMeshRenderer.material = closestEnemyMat;

        //         closestEnemies.Add(closestEnemy);

        //         //Move friend towards enemy :c
        //         friendlySoldiers[friend].Move(closestEnemy);
        //     }
        // }
    }

    Soldier FindClosestEnemy(Soldier friend)
    {
        // Soldier closestEnemy = null;

        // float bestDistSqr = Mathf.Infinity;

        // //Loop through all enemies
        // for (int i = 0; i < enemySoldiers.Count; i++)
        // {
        //     //Dst-sqr btwn enemy & friend
        //     float distSqr = (friend.soldierTrans.position - enemySoldiers[i].soldierTrans.position).sqrMagnitude;

        //     //If ^ dist is better than previous, found a closer enemy
        //     if (distSqr < bestDistSqr)
        //     {
        //         bestDistSqr = distSqr;

        //         closestEnemy = enemySoldiers[i];
        //     }
        // }
        // return closestEnemy;
        return new Soldier();
    }
}
