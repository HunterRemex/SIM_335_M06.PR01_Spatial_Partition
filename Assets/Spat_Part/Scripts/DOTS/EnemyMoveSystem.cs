using Runtime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine; //Logging

[AlwaysUpdateSystem] //Run on tick
// [UpdateInGroup(typeof(ComponentSystemGroup))]
[UpdateBefore (typeof (EnemyUpdateGrid))]

public class EnemyMoveSystem : SystemBase {

    [NativeDisableParallelForRestrictionAttribute]
    ComponentDataFromEntity<Enemy> enemyComps;
    protected override void OnStartRunning () {

        enemyComps = GetComponentDataFromEntity<Enemy> ();
        #region  Startup Enemies

        var randArr = World.GetExistingSystem<RandomSystem> ().Rands;

        var enemies = enemyComps;
        /// <summary>
        /// Place Enemies around the map
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        Entities
            .WithNativeDisableParallelForRestriction (randArr) //k
            .ForEach ((int nativeThreadIndex, ref Enemy enemy, ref Translation tx, ref Rotation rot, ref TargetPosition tPos, in Entity entity) => {
                var lRand = randArr[nativeThreadIndex]; //Get randoms

                //Place them somewhere
                tx.Value = new float3 (lRand.NextFloat (SoldierSpawner.MapWidth), lRand.NextFloat (SoldierSpawner.MapWidth), lRand.NextFloat (SoldierSpawner.MapWidth));

                enemy.oldPos = tx.Value;

                //Point them to go somewhere
                tPos.Value = new float3 (lRand.NextFloat (SoldierSpawner.MapWidth), lRand.NextFloat (SoldierSpawner.MapWidth), lRand.NextFloat (SoldierSpawner.MapWidth));
                rot.Value = UnityEngine.Quaternion.LookRotation (tPos.Value - tx.Value);



                int soldierToInform = Grid.instance.Add (enemy.soldierData, tx.Value);



                randArr[nativeThreadIndex] = lRand; //Update randoms

            }).WithoutBurst ().Run (); //  .WithStructuralChanges().WithoutBurst().Run(); //

        #endregion Startup Enemies
    }

    protected override void OnUpdate () {
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        float deltaTime = Time.DeltaTime;
        var randArr = World.GetExistingSystem<RandomSystem> ().Rands;

        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.

        //Filter to Enemy soldiers

        Entities
            .WithNativeDisableParallelForRestriction (randArr) //k
            .ForEach ((int nativeThreadIndex, Entity entity, ref TargetPosition tPos, ref Translation tx, ref Rotation rot, ref Enemy en) => {
                var rand = randArr[nativeThreadIndex];

                ///Give the guy a new target pos on the grid
                if (((Vector3) tx.Value - (Vector3) (tPos.Value)).magnitude <= 1f) //If near the target pos
                {
                    float3 _xyzPos = rand.NextFloat3 (SoldierSpawner.MapWidth);

                    float3 _dir = rand.NextFloat3Direction ();
                    _dir *= SoldierSpawner.MapWidth / 2.25f; //Make the sphere a bit smaller than grid bounds
                    _dir += SoldierSpawner.MapWidth / 2f; //Move the sphere to the center of the grid
                    _xyzPos = _dir;

                    tPos.Value = new float3 (_xyzPos.x, _xyzPos.y, _xyzPos.z);
                    rot.Value = Quaternion.LookRotation (tPos.Value - tx.Value);
                }

                var _oldPos = tx.Value;

                // Debug.Log("Got Here"); // \/ Auto-convert is just too hard, isn't it
                tx.Value += (float3) ((Quaternion) rot.Value * Vector3.forward) * en.soldierData.walkSpeed * deltaTime;

                en.oldPos = _oldPos;

                en = new Enemy {
                    oldPos = _oldPos,

                    soldierData = new Soldier {
                    entityId = en.soldierData.entityId,
                    previousSoldier = en.soldierData.previousSoldier,
                    nextSoldier = en.soldierData.nextSoldier,
                    walkSpeed = en.soldierData.walkSpeed,
                    position = tx.Value,
                    }
                };

                //Not running always for some forsen reason
                //For GridSys: Allows friendlies to find this guys pos simply (Don't have to figure out how to pass in Enemy)

                randArr[nativeThreadIndex] = rand; //Update randoms

            }).WithBurst ().ScheduleParallel (); //SP Does work .WithoutBurst().Run();//
    }
}