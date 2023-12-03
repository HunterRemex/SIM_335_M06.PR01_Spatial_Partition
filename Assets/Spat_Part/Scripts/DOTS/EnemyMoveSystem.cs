using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

[UpdateBefore(typeof(EnemyUpdateGrid))]
[RequireMatchingQueriesForUpdate]
public partial class EnemyMoveSystem : SystemBase {

    [NativeDisableParallelForRestriction]
    ComponentLookup<Enemy> enemyComps;
    protected override void OnStartRunning () {

        enemyComps = GetComponentLookup<Enemy> ();

        #region Startup Enemies

        var randArr = World.GetExistingSystemManaged<RandomSystem>().Rands;

        ComponentLookup<Enemy> enemies = enemyComps;

        Entities.WithNativeDisableParallelForRestriction(randArr)
                .WithName("Enemy_Positions_Initialize")
                .WithAll<LocalToWorld>()
                .ForEach((int entityInQueryIndex, ref Enemy enemy, ref LocalTransform tx, ref TargetPosition tPos, in Entity entity) => {
                    var lRand = randArr[entityInQueryIndex]; //Get randoms

                    //Place them somewhere
                    tx.Position = new float3 (lRand.NextFloat (SoldierSpawner.MapWidth), lRand.NextFloat (SoldierSpawner.MapWidth), lRand.NextFloat (SoldierSpawner.MapWidth));

                    enemy.oldPos = tx.Position;

                    // Point them to go somewhere
                    tPos.Value = new float3 (lRand.NextFloat (SoldierSpawner.MapWidth), lRand.NextFloat (SoldierSpawner.MapWidth), lRand.NextFloat (SoldierSpawner.MapWidth));
                    tx.Rotation = Quaternion.LookRotation (tPos.Value - tx.Position);


                    int soldierToInform = Grid.instance.Add(enemy.soldierData, tx.Position);

                    randArr[entityInQueryIndex] = lRand; //Update randoms

                }).WithoutBurst().WithStructuralChanges().Run();

        #endregion Startup Enemies
    }

    protected override void OnUpdate () {
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        float deltaTime = World.Time.DeltaTime;
        var randArr = World.GetExistingSystemManaged<RandomSystem>().Rands;

        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.

        //Filter to Enemy soldiers

        Entities.WithNativeDisableParallelForRestriction (randArr)
                .WithName("Enemy_Positions_Update")
                .ForEach ((int nativeThreadIndex, Entity entity, ref TargetPosition tPos, ref LocalTransform tx, ref Enemy en) => {
                    var rand = randArr[nativeThreadIndex];

                    //Give the guy a new target pos on the grid
                    if (((Vector3) tx.Position - (Vector3) (tPos.Value)).magnitude <= 1f) //If near the target pos
                    {
                        float3 _xyzPos = rand.NextFloat3 (SoldierSpawner.MapWidth);

                        float3 _dir = rand.NextFloat3Direction ();
                        _dir *= SoldierSpawner.MapWidth / 2.25f; //Make the sphere a bit smaller than grid bounds
                        _dir += SoldierSpawner.MapWidth / 2f; //Move the sphere to the center of the grid
                        _xyzPos = _dir;

                        tPos.Value = new float3 (_xyzPos.x, _xyzPos.y, _xyzPos.z);
                        tx.Rotation = Quaternion.LookRotation (tPos.Value - tx.Position);
                    }

                    var _oldPos = tx.Position;

                    // Debug.Log("Got Here"); // \/ Auto-convert is just too hard, isn't it
                    tx.Position += (float3) ((Quaternion) tx.Rotation * Vector3.forward) * en.soldierData.walkSpeed * deltaTime;

                    en.oldPos = _oldPos;

                    en = new Enemy {
                        oldPos = _oldPos,

                        soldierData = new Soldier {
                            entityId = en.soldierData.entityId,
                            previousSoldier = en.soldierData.previousSoldier,
                            nextSoldier = en.soldierData.nextSoldier,
                            walkSpeed = en.soldierData.walkSpeed,
                            position = tx.Position,
                        }
                    };

                    //Not running always for some reason
                    //For GridSys: Allows friendlies to find this guys pos simply (Don't have to figure out how to pass in Enemy)

                    randArr[nativeThreadIndex] = rand; //Update randoms

                }).WithBurst ().ScheduleParallel (); //SP Does work .WithoutBurst().Run();//
    }
}