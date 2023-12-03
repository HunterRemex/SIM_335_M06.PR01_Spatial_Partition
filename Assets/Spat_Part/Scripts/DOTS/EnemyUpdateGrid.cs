using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(EnemyMoveSystem))]
[UpdateBefore(typeof(FriendlyMoveSystem))]
public partial class EnemyUpdateGrid : SystemBase
{
	public static NativeArray<Entity> enemyEntities;

	/// <summary>
	/// Prep the query, collect entities
	/// </summary>
	EntityQuery m_EnemyEQ;

	protected override void OnCreate()
	{
		var enemyQuery = new EntityQueryDesc
		{
			All = new ComponentType[]
			{
				ComponentType.ReadWrite<Enemy>(),
				ComponentType.ReadWrite<TargetPosition>(),
				ComponentType.ReadOnly<LocalTransform>()
			}
		};

		m_EnemyEQ = GetEntityQuery(enemyQuery);
	}

	/// <summary>
	/// Prepare to fire query
	/// </summary>
	/// <param name="jobHandle"></param>
	/// <returns></returns> 
	protected override void OnUpdate()
	{
		// enemyEntities.Dispose();

		var enemyChunks = m_EnemyEQ.ToArchetypeChunkArray(Allocator.Temp);

		var entType = GetEntityTypeHandle();
		var enemyType = GetComponentTypeHandle<Enemy>();
		var targetType = GetComponentTypeHandle<TargetPosition>();
		var txType = GetComponentTypeHandle<LocalTransform>();

		//If error here: Disable Jobs -> Mem Leak Detection
		// enemyEntities = m_EnemyEQ.ToEntityArray (Allocator.Persistent);

		var job = new EnemyUpdateGridJob
		{
			entity = entType,
			enemyTarget = targetType,
			tx = txType,
			enemies = this.GetComponentLookup<Enemy>(),
			// jobHandle = jobHandle,
			enemyEntities = GridJobStatics.enemyEntities,
		};

		// return job.ScheduleParallel(m_EnemyEQ, null);
	}

	// <summary>
	// Fire query alterations
	// </summary>
	// [BurstCompile] //UNABLE: references Managed-code Grid.cs
	struct EnemyUpdateGridJob : IJobChunk
	{
		[ReadOnly] public EntityTypeHandle entity;
		[ReadOnly] public NativeArray<Entity> enemyEntities;

		public ComponentTypeHandle<TargetPosition> enemyTarget;
		public ComponentTypeHandle<LocalTransform> tx;

		[NativeDisableParallelForRestrictionAttribute]
		public ComponentLookup<Enemy> enemies;

		public JobHandle jobHandle;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//Get chunks to process
			var chunkEntity = chunk.GetNativeArray(entity);
			var chunkTargetPos = chunk.GetNativeArray(ref enemyTarget);
			var chunkTx = chunk.GetNativeArray(ref tx);

			// Loop through all enemies
			for (int _enemyNum = 0; _enemyNum < chunkEntity.Length; _enemyNum++)
			{
				Enemy _me = enemies[chunkEntity[_enemyNum]];


				// <summary>
				// Bounds check in case a lad escapes
				// </summary>
				// <param name="SoldierSpawner.cellSize"></param>
				// <returns></returns>
				int oldCellX = math.min((int) (_me.oldPos.x / SoldierSpawner.CellSize),
					Grid.instance.numberOfCells - 1);
				int oldCellY = math.min((int) (_me.oldPos.y / SoldierSpawner.CellSize),
					Grid.instance.numberOfCells - 1);
				int oldCellZ = math.min((int) (_me.oldPos.z / SoldierSpawner.CellSize),
					Grid.instance.numberOfCells - 1);

				int cellX = math.min((int) (_me.soldierData.position.x / SoldierSpawner.CellSize),
					Grid.instance.numberOfCells - 1);
				int cellY = math.min((int) (_me.soldierData.position.y / SoldierSpawner.CellSize),
					Grid.instance.numberOfCells - 1);
				int cellZ = math.min((int) (_me.soldierData.position.z / SoldierSpawner.CellSize),
					Grid.instance.numberOfCells - 1);
				bool inSameCell = (oldCellX == cellX) && (oldCellY == cellY) && (oldCellZ == cellZ);

				if (inSameCell)
				{
					//Don't need to update 
					continue;
				}

				///Otherwise, We're leaving this cell, tell the others

				// <summary>
				// Inform the person behind us that we're leaving and they should look to the person ahead of us
				// </summary>
				if (_me.soldierData.previousSoldier != -1)
				{
					// Debug.LogFormat ("next: {0} while us: {1}", _me.soldierData.previousSoldier, _me.soldierData.entityId);

					//Save a copy of the guy behind us
					Soldier _enBehindUs = enemies[new Entity {Index = _me.soldierData.previousSoldier, Version = 1}]
						.soldierData;
					//Edit the live version of the guy behind us
					enemies[new Entity {Index = _me.soldierData.previousSoldier, Version = 1}] = new Enemy
					{
						oldPos = enemies[new Entity {Index = _me.soldierData.previousSoldier, Version = 1}].oldPos,

						soldierData = new Soldier
						{
							entityId = _enBehindUs.entityId,
							previousSoldier = _enBehindUs.previousSoldier,

							nextSoldier = _me.soldierData.nextSoldier,

							walkSpeed = _enBehindUs.walkSpeed,
							position = _enBehindUs.position
						}
					};
				}

				// <summary>
				// Inform the person ahead us that we're leaving and they should look to the person behind of us
				// </summary>
				if (_me.soldierData.nextSoldier != -1)
				{
					Soldier _enAheadUs = enemies[new Entity {Index = _me.soldierData.nextSoldier, Version = 1}]
						.soldierData;
					enemies[new Entity {Index = _me.soldierData.nextSoldier, Version = 1}] = new Enemy
					{
						oldPos = enemies[new Entity {Index = _me.soldierData.nextSoldier, Version = 1}].oldPos,

						soldierData = new Soldier
						{
							entityId = _enAheadUs.entityId,

							previousSoldier = _me.soldierData.previousSoldier,

							nextSoldier = _enAheadUs.nextSoldier,
							walkSpeed = _enAheadUs.walkSpeed,
							position = _enAheadUs.position
						}
					};
				}

				// <summary>
				// Inform the grid that we're leaving if we're the head/leader of the cell
				// </summary>
				if (Grid.instance.data.cells[oldCellX, oldCellY, oldCellZ].entityId == _me.soldierData.entityId &&
				    _me.soldierData.nextSoldier != -1)
				{
					Grid.instance.data.cells[oldCellX, oldCellY, oldCellZ] = enemies[new Entity {Index = _me.soldierData.nextSoldier, Version = 1}].soldierData;
				}


				//Add ourselves back to the grid
				int soldierToInform = Grid.instance.Add(enemies[chunkEntity[_enemyNum]].soldierData, chunkTx[_enemyNum].Position);

				if (soldierToInform != -1)
				{
					//Inform ourselves of the person ahead
					enemies[chunkEntity[_enemyNum]] = new Enemy
					{
						oldPos = _me.oldPos,

						soldierData = new Soldier
						{
							entityId = _me.soldierData.entityId,
							previousSoldier = _me.soldierData.previousSoldier,

							nextSoldier = soldierToInform,

							position = _me.soldierData.position,
							walkSpeed = _me.soldierData.walkSpeed
						}
					};

					//Inform person ahead that we're behind them
					// Debug.LogFormat ("Trying to inform {0} from {1}", soldierToInform, _me.soldierData.entityId);
					Enemy _enemyAhead = enemies[new Entity {Index = soldierToInform, Version = 1}];
					enemies[new Entity {Index = soldierToInform, Version = 1}] = new Enemy
					{
						oldPos = _enemyAhead.oldPos,

						soldierData = new Soldier
						{
							entityId = _enemyAhead.soldierData.entityId,

							previousSoldier = _me.soldierData.entityId,

							nextSoldier = _enemyAhead.soldierData.nextSoldier,
							walkSpeed = _enemyAhead.soldierData.walkSpeed,
							position = _enemyAhead.soldierData.position
						}
					};
				}
			}
		}
	}
}