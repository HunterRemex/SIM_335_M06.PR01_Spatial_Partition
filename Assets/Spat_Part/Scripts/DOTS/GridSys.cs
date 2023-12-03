using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(SoldierSpawner))]
[UpdateAfter(typeof(EnemyUpdateGrid))]
public partial class GridSys : SystemBase
{
	public Grid grid;
	private static EntityManager entManager;

	private static EndSimulationEntityCommandBufferSystem _endSimEcbSystem;

	protected override void OnCreate()
	{
		grid = Grid.instance; //Singleton
		entManager = World.DefaultGameObjectInjectionWorld.EntityManager;

		_endSimEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	}

	protected override void OnStartRunning()
	{
		var enemies = GetComponentDataFromEntity<Enemy>();

		// <summary>
		// Add Enemies onto the grid and (sufferingly) create a linked-ish list
		// </summary>
		// <param name="entity"></param>
		// <param name="soldier"></param>
		// <param name="translation"></param>
		// <param name="enemy"></param> Filter param
		// <returns></returns>
		Entities
			.ForEach((Entity entity, ref Enemy en, in Translation translation) =>
			{
				//Add soldier to the managed grid

				// Debug.LogFormat ("Informing");//Runs correct amnt of times

				//Init for add to grid
				en.soldierData = new Soldier
				{
					entityId = entity.Index,
					previousSoldier = -1,
					nextSoldier = -1,
					walkSpeed = 12
				};


				var soldierToInform = grid.Add(en.soldierData, translation.Value);

				if (soldierToInform <= 0) return;
				//Inform ourselves of who's ahead of us
				//Following Grid guide, null out prev.soldier & note ahead
				en.soldierData = new Soldier
				{
					entityId = entity.Index,
					previousSoldier = -1,
					nextSoldier = soldierToInform,
					walkSpeed = 13
				};

				//Inform person ahead that we're behind them
				// Debug.LogFormat ("Trying to inform {0} from {1}", soldierToInform, en.soldierData.entityId);
				Enemy _enemyAhead = enemies[new Entity {Index = soldierToInform, Version = 1}];
				enemies[new Entity {Index = soldierToInform, Version = 1}] = new Enemy
				{
					oldPos = _enemyAhead.oldPos,

					soldierData = new Soldier
					{
						entityId = _enemyAhead.soldierData.entityId,

						previousSoldier = en.soldierData.entityId,

						nextSoldier = _enemyAhead.soldierData.nextSoldier,
						walkSpeed = 14,
						position = _enemyAhead.soldierData.position
					}
				};
			}).WithoutBurst().Run();
	}

	protected override void OnUpdate()
	{
	}

	public static List<Entity> addedEnemies = new List<Entity>();
	private static List<Entity> addedEnemiesDisabled = new List<Entity>();
	public static void AddEnemyToGridLate(int numberToAdd)
	{
		if (addedEnemiesDisabled.Count > 0)
		{//We have disabled enemies we can just re-enable

			// for (int numToAdd = 0; numToAdd < numberToAdd; numToAdd++)
			{
				// var enemyToAddBack = addedEnemiesDisabled.First();
				// addedEnemiesDisabled.Remove(enemyToAddBack);
				// entManager.SetEnabled(enemyToAddBack, true);
				// addedEnemies.Add(enemyToAddBack);
			}
		}
		for (int i = 0; i < numberToAdd; i++)
		{
			var enmy = entManager.Instantiate(SoldierSpawner.FirstEnemyLad);

			var tx = entManager.GetComponentData<Translation>(enmy);
			var tPos = new float3(50f, 50f, 50f);

			entManager.AddComponentData(enmy, new Enemy
			{
				oldPos = new float3(0, 0, 0),
				soldierData = new Soldier
				{
					entityId = enmy.Index,
					previousSoldier = -1,
					nextSoldier = -1,
					walkSpeed = 12
				}
			});

			entManager.AddComponentData(enmy, new TargetPosition
			{
				Value = tPos //Should be SoldierSpawner.mapWidth into an lRand, don't want to bother pulling in lRand
			});
			entManager.SetComponentData(enmy, new Rotation
			{
				Value = Quaternion.LookRotation(tPos - tx.Value)
			});

			// <summary>
			// Materialization
			// </summary>
			entManager.AddComponentData(enmy, new EnemyMaterialColor { });

			entManager.SetEnabled(enmy, true); //Incase disabled by below
			addedEnemies.Add(enmy);

			// Debug.LogFormat("Added {0}", enmy.Index);
		}
	}

	public static void RemoveEnemyFromGridLate()
	{
		var ecb = _endSimEcbSystem.CreateCommandBuffer();
		// Debug.Log("Pulling");
		// entManager.SetEnabled(soldiers[0], false);
		if (addedEnemies.Count > 0)
		{
			// var lastEnemy = addedEnemies.Last();
			// addedEnemies.Remove(lastEnemy);
			// ecb.DestroyEntity(lastEnemy);
			// addedEnemiesDisabled.Add(lastEnemy);
			// entManager.SetEnabled(lastEnemy, false);
			// entManager.DestroyEntity(addedEnemies.Last()); //Causes 'entity does not exist' issues
		}
		
	}
}