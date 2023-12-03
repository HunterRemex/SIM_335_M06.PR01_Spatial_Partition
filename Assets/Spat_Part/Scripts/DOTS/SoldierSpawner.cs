using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;

[UpdateBefore(typeof(GridSys))]
public partial class SoldierSpawner : SystemBase
{
	//Grid Data
	public static readonly int MapWidth = 100;
	public static readonly int CellSize = 5;


	//Num soldiers per team
	public readonly static int NumSoldiersPerTeam = 3500;

	//The grid
	public Grid grid = new Grid(MapWidth, CellSize);


	/// <summary>
	/// Dots Stuff
	/// </summary>
	private Unity.Mathematics.Random _rand;

	public static NativeArray<Enemy> EnemyComponents;

	public static Entity FirstEnemyLad;

	protected override void OnCreate()
	{
	}


	protected override void OnStartRunning()
	{
		_rand = new Unity.Mathematics.Random(140621);
		var lRand = _rand;

		NativeArray<Entity> enemyInstances = new NativeArray<Entity>(NumSoldiersPerTeam, Allocator.Temp);
		NativeArray<Entity> friendInstances = new NativeArray<Entity>(NumSoldiersPerTeam, Allocator.Temp);


		// Material closeMaterial = Resources.Load("MAT_Close", typeof(Material)) as Material;
		// Material farMaterial = Resources.Load("MAT_Far_Enemy", typeof(Material)) as Material;
		
		Entities
			.ForEach((PrefabEntityComponent prefEntComp) =>
			{
				//Instantiate enemies
				EntityManager.Instantiate(prefEntComp.enemyPrefab, enemyInstances);
				//Instantiate friends
				EntityManager.Instantiate(prefEntComp.friendPrefab, friendInstances);
			}).WithStructuralChanges().Run();


		// Add Friendly to friends
		foreach (Entity fren in friendInstances)
		{
			EntityManager.AddComponentData(fren, new Friendly
			{
				soldierData = new Soldier
				{
					entityId = fren.Index,
					walkSpeed = 24f
				}
			});
			EntityManager.AddComponentData(fren, new TargetPosition { });
		}


		//  For enemy entities, add the relevant scripts, and ID them for the GRID
		foreach (Entity enmy in enemyInstances)
		{
			EntityManager.AddComponentData(enmy, new Enemy
			{
				soldierData = new Soldier
				{
				}
			});

			EntityManager.AddComponentData(enmy, new TargetPosition { });

			// <summary>
			// Materialization
			// </summary>
			EntityManager.AddComponentData(enmy,
				new EnemyMaterialColor
				{
					Value = new float4(0.8f, 0.518f, 0, 1)
				});

		}

		FirstEnemyLad = enemyInstances[0];


		//Place Friends around the map
		Entities
			.ForEach((Entity soldier, ref LocalTransform tx, ref Friendly fren) => //Filter to entities with a Soldier component (Friends & Enemies)
				{
					//Place them somewhere
					tx.Position = new float3(lRand.NextFloat(MapWidth), lRand.NextFloat(MapWidth),
						lRand.NextFloat(MapWidth)); 
				}).WithBurst().ScheduleParallel(); //.WithStructuralChanges().Run();
	}


	protected override void OnUpdate()
	{
	}
}