using Unity.Collections;
using Unity.Entities;
using Unity.Jobs.LowLevel.Unsafe;


public partial class GridJobStatics : SystemBase
{
	public static NativeArray<Entity> enemyEntities;


	/// <summary>
	/// Prep the query, collect entities
	/// </summary>
	EntityQuery m_EnemyEQ;

	protected override void OnStartRunning()
	{
		var enemyQuery = new EntityQueryDesc
		{
			All = new ComponentType[] {ComponentType.ReadWrite<Enemy>()}
		};

		m_EnemyEQ = GetEntityQuery(enemyQuery);

		enemyEntities = m_EnemyEQ.ToEntityArray(Allocator.Persistent);
	}

	protected override void OnUpdate()
	{
		// Debug.Log("Updated Statics: " + enemyEntities.Length);
	}
}


/// <summary>
/// From: https://reeseschultz.com/random-number-generation-with-unity-dots/
/// </summary>
[UpdateInGroup(typeof(InitializationSystemGroup))]
internal partial class RandomSystem : ComponentSystemBase
{
	public NativeArray<Unity.Mathematics.Random> Rands;

	protected override void OnCreate()
	{
		var _randArray = new Unity.Mathematics.Random[JobsUtility.MaxJobThreadCount];
		var _seed = new System.Random();

		for (int _i = 0; _i < JobsUtility.MaxJobThreadCount; _i++)
		{
			_randArray[_i] = new Unity.Mathematics.Random((uint) _seed.Next());
		}

		Rands = new NativeArray<Unity.Mathematics.Random>(_randArray, Allocator.Persistent);
	}

	protected override void OnDestroy() => Rands.Dispose();

	protected override void OnUpdate()
	{
	}
}