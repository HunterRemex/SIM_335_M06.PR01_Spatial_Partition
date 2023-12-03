using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Find nearest enemy to go towards for a Friendly agent
/// </summary>
[UpdateAfter (typeof (GridJobStatics))]
[UpdateBefore (typeof (SoldierSpawner))]
public class FriendlyUpdateGrid : ComponentSystemBase {

    /// <summary>
    /// Prep the query
    /// </summary>
    EntityQuery m_FriendlyEQ;

    protected override void OnCreate () {
        var friendQuery = new EntityQueryDesc {
            All = new ComponentType[] { ComponentType.ReadOnly<Friendly> () }
        };

        m_FriendlyEQ = GetEntityQuery (friendQuery);

    }

    /// <summary>
    /// Prepare to fire query alter
    /// </summary>
    /// <param name="jobHandle"></param>
    /// <returns></returns>

    protected override JobHandle OnUpdate (JobHandle jobHandle) {
        var friendlyType = GetComponentTypeHandle<Friendly> (); //Should change to friendly
        var targetType = GetComponentTypeHandle<TargetPosition> ();

        var job = new FindClosestEnemyJob {
            friendlySoldier = friendlyType,
            friendlyTarget = targetType,

            enemyEntities = GridJobStatics.enemyEntities,
            enemies = this.GetComponentDataFromEntity<Enemy> (),
            enMat = this.GetComponentDataFromEntity<EnemyMaterialColor> (),
        };
        return job.ScheduleParallel (m_FriendlyEQ, jobHandle);
    }

    /// <summary>
    /// Fire query alterations
    /// </summary>
    // [BurstCompile]
    struct FindClosestEnemyJob : IJobChunk {
        [ReadOnly]
        public NativeArray<Entity> enemyEntities;

        public ComponentTypeHandle<Friendly> friendlySoldier;
        public ComponentTypeHandle<TargetPosition> friendlyTarget;

        [NativeDisableParallelForRestrictionAttribute]
        public ComponentDataFromEntity<EnemyMaterialColor> enMat;

        [ReadOnly]
        public ComponentDataFromEntity<Enemy> enemies;

        public void Execute (ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex) {
            // enemyEntities = GridJobStatics.enemyEntities;
            //Get chunks to process //Fails BURST
            var chunkFriend = chunk.GetNativeArray (friendlySoldier);

            var chunkTargetPos = chunk.GetNativeArray (friendlyTarget);

            //_friendNum used to be firstEntityIndex. Only worked for first chunk. Purpose of var?
            /// Loop through all friendlies
            for (int _friendNum = 0; _friendNum < chunkFriend.Length; _friendNum++) {

                var _ourPos = chunkFriend[_friendNum].soldierData.position;

                //Get initial enemy
                int _cellX = math.min ((int) (_ourPos.x / Grid.instance.data.cellSize), Grid.instance.numberOfCells - 1);
                int _cellY = math.min ((int) (_ourPos.y / Grid.instance.data.cellSize), Grid.instance.numberOfCells - 1);
                int _cellZ = math.min ((int) (_ourPos.z / Grid.instance.data.cellSize), Grid.instance.numberOfCells - 1); //Bounds checks should be moved to the Grid's getter

                Soldier enemy = Grid.instance.data.cells[_cellX, _cellY, _cellZ];

                Soldier closestSoldier = new Soldier { };

                float bestDistSqr = Mathf.Infinity;

                float3 _frPos = chunkFriend[_friendNum].soldierData.position;

                int _maxIter = 150;
                int _iter = 0;
                #region linked
                while (enemy.nextSoldier > 0 && (_iter < _maxIter)) {
                    float3 _enPos = enemy.position;

                    float3 _dist = _enPos - _frPos;
                    float distSqr = _dist.x * _dist.x + _dist.y * _dist.y + _dist.z * _dist.z;

                    if (distSqr < bestDistSqr) {
                        bestDistSqr = distSqr;

                        closestSoldier = enemy;
                    }

                    // Debug.LogFormat ("Checking {0}", enemy.nextSoldier);
                    
                    enemy = enemies[new Entity { Index = enemy.nextSoldier, Version = 1 }].soldierData;
                    _iter++;
                }
                #endregion linked

                //Find the closest enemy (not Linked-Listed)
                #region non-linked
                // foreach (Soldier soldier in Grid.instance.data.cells)
                // {
                //     float3 _enPos = soldier.position;

                //     float3 _dist = _enPos - _frPos;
                //     float distSqr = _dist.x * _dist.x + _dist.y * _dist.y + _dist.z * _dist.z;

                //     if (distSqr < bestDistSqr)
                //     {
                //         bestDistSqr = distSqr;

                //         closestEntity = enemyEntities[soldier.nextSoldier];
                //         // var _enmy = enemies[_ent];

                //         closestSoldier = enemies[enemyEntities[soldier.nextSoldier]].soldierData;

                //         // if(chunkFriend[_friendNum].soldierData.entityId > 210)
                //         {
                //         // Debug.LogFormat("Found enemy {0} setting target to {1} with us {2}", _ent.Index, _enmy.soldierData.position, chunkFriend[_friendNum].soldierData.entityId);
                //             // Debug.LogFormat("Zero'd target while our ID: {0}", chunkFriend[_friendNum].soldierData.entityId);
                //         }
                //     }

                // }
                /// TD: check discrepancy between actual enemy pos and reported, lads still don't go where they should /// Correct, but they still bunch up @ center
                // Debug.LogFormat("Found best Sqr of {0} on enemy {1}", bestDistSqr, closestSoldier.id);
                #endregion non-linked

                // Debug.LogFormat ("Looking towards enmat on ID {0}", enemy.entityId);
                if (enemy.entityId > 0) {
                    enMat[new Entity { Index = enemy.entityId, Version = 1 }] = new EnemyMaterialColor {
                        Value = new float4 (0, 1, 0, 1)
                    };
                }
                chunkTargetPos[_friendNum] = new TargetPosition { Value = closestSoldier.position };

            }
        }

    }

}