using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	public GameObject[] spawnPortals;
	public GameObject[] enemyPrefabs;
	public GameObject dronePrefab;
	public float enemyHealthMultiplier;
	public float enemyArmorMultiplier;
	public float droneArmorMultiplier;
	public float spawnRate = 1.0f;
	public int enemyCount;

	private PlayerController playerController;
	private GameManager gameManager;
	private TransitionsHandler transitionsHandler;
	private Vector3 spawnPos;
	private float spawnOffset = 2.3f;
	private bool canSpawn = true;

	// Start is called before the first frame update
	void Start()
	{
		playerController = GameObject.Find("Player").GetComponent<PlayerController>();
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
		transitionsHandler = GameObject.Find("Loading Crossfade").GetComponent<TransitionsHandler>();

		gameManager.StartGame();
	}

	private void IncreaseEnemiesDifficulty()
	{
		gameManager.enemyWaveStats[0] = gameManager.waveNumber * enemyHealthMultiplier;
		gameManager.enemyWaveStats[1] = gameManager.waveNumber * enemyArmorMultiplier;

		gameManager.droneWaveStat = gameManager.waveNumber * droneArmorMultiplier;
	}

	// Update is called once per frame
	void Update()
	{
		enemyCount = FindObjectsOfType<EnemyController>().Length +
			FindObjectsOfType<DroneController>().Length;

		if (enemyCount == 0 && gameManager.isGameActive)
		{
			if (gameManager.waveNumber > 0 && !gameManager.skip)
			{
				gameManager.wavesCleared++;
				playerController.UpdateScore();
				IncreaseEnemiesDifficulty();

				gameManager.buyBreak = true;
			}

			if (gameManager.skip)
				gameManager.skip = false;

			if (gameManager.buyBreak)
			{
				gameManager.skip = true;
				transitionsHandler.TransitionTo("Buying Place");
			}

			if (!gameManager.buyBreak && canSpawn)
			{
				gameManager.waveNumber++;
				StartCoroutine(SpawnEnemyWave(gameManager.waveNumber));

				if ((gameManager.waveNumber % 4) == 0)
					StartCoroutine(SpawnDrone(gameManager.waveNumber/4));
			}
		}
	}

	private IEnumerator SpawnEnemyWave(int enemiesToSpawn)
	{
		for (int i = 0; i < enemiesToSpawn; i++)
		{
			if (!gameManager.isGameActive)
				break;

			canSpawn = false;

			transform.rotation = Quaternion.identity;

			int randomSpawn = Random.Range(0, spawnPortals.Length);
			int enemyIndex = Random.Range(0, 3);

			GameObject portal = spawnPortals[randomSpawn];

			Vector3 portalPos = portal.transform.position;
			Vector3 spawnPos = new Vector3(portalPos.x, transform.position.y, portalPos.z);

			PortalBehaviour portalController = portal.GetComponent<PortalBehaviour>();
			portalController.ChangeStateTo("active");

			GameObject enemy = Instantiate(
				enemyPrefabs[enemyIndex],
				spawnPos,
				portal.transform.rotation
			);
			Vector3 offsettedPos = enemy.transform.forward * spawnOffset;
			enemy.transform.position -= offsettedPos;
			EnemyController enemyController = enemy.GetComponent<EnemyController>();

			yield return new WaitForSeconds(spawnRate);
			portalController.ChangeStateTo("hidden");
			canSpawn = true;
		}
	}

	private IEnumerator SpawnDrone(int dronesToSpawn)
	{
		for (int i = 0; i < dronesToSpawn; i++)
		{
			float xSpawn = Random.Range(-25.0f, -20.0f);
			float zSpawn = Random.Range(9.0f, 80.0f);

			Instantiate(
				dronePrefab,
				new Vector3(xSpawn, dronePrefab.transform.position.y, zSpawn),
				dronePrefab.transform.rotation
			);

			yield return new WaitForSeconds(spawnRate);
		}
	}
}
