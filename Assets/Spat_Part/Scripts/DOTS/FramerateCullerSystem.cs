using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using TMPro;

// [AlwaysUpdateSystem]
[UpdateAfter(typeof(GridSys))]
[UpdateAfter(typeof(GridJobStatics))]
public class FramerateMatcherSystem : SystemBase
{
	public float targetFrameRate = 80f;
	private float targetFrameTime;

	public TextMeshProUGUI enemyCountGUI;
	public TextMeshProUGUI targetFPSGUI;
	public TextMeshProUGUI enemyPerMSGUI;

	protected override void OnCreate()
	{
		Debug.Log("looking for UI's");
		enemyCountGUI = UnityEngine.GameObject.Find("EnemyCountGUI").GetComponent<TextMeshProUGUI>();
		targetFPSGUI = UnityEngine.GameObject.Find("TargetFPSGUI").GetComponent<TextMeshProUGUI>();
		enemyPerMSGUI = UnityEngine.GameObject.Find("EnemyPerMSGUI").GetComponent<TextMeshProUGUI>();

		Debug.Log("finished looking");

		targetFrameTime = (1000f / targetFrameRate);
	}

	protected override void OnUpdate()
	{
		float currentFrameTime = UnityEngine.Time.smoothDeltaTime * 1000f; //Smoothed unavailable in DOTS

		var totalEnemyCount = SoldierSpawner.NumSoldiersPerTeam + GridSys.addedEnemies.Count;

		enemyCountGUI.text = "Enemy Count: " + totalEnemyCount;
		targetFPSGUI.text = "Targeting: " + targetFrameRate + " FPS @ " + targetFrameTime +
		                    "ms per frame \nSmoothed reporting " + currentFrameTime.ToString("F3") + "ms";
		enemyPerMSGUI.text = "Each enemy approx " + (currentFrameTime / totalEnemyCount).ToString("F5") + "ms";
		Debug.Log("updating UI");


		if (targetFrameTime > currentFrameTime) //Frame is taking shorter than requested, spawn more guys
		{
			GridSys.AddEnemyToGridLate(5);
		}
		else if (targetFrameTime < currentFrameTime) //Frame is taking longer than desired, remove a lad
		{
			GridSys.RemoveEnemyFromGridLate();
		}
	}
}