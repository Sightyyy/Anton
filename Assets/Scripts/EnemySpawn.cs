using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject normalMonster;
    public GameObject speedMonster;
    public GameObject tankMonster;
    public GameObject bossMonster;

    [Header("Wave Settings")]
    public int currentWave = 1;
    public int maxWave = 25;
    public int spawnMultiplier = 1;

    private int normalCount = 5;
    private int speedCount = 2;
    private int tankCount = 1;
    private int bossCount = 0;

    [Header("Spawn Settings")]
    public float minSpawnDistance = 15f;
    public float maxSpawnDistance = 45f;
    public float spawnDelay = 0.15f;
    public float waveStartDelay = 2f;

    public Transform player;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private bool isSpawning = false;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player not assigned to EnemySpawn!");
            return;
        }

        StartCoroutine(SpawnWave());
    }

    private void Update()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);

        if (!isSpawning && spawnedEnemies.Count == 0)
        {
            currentWave++;
            Debug.Log("Wave cleared! Starting next wave: " + currentWave);
            StartCoroutine(SpawnWave());
        }
    }

    public void WaveManagement()
    {
        int x = currentWave;

        if ((x - 1) % 30 == 0)
        {
            spawnMultiplier++;
        }

        if (x % 10 == 0)
        {
            normalCount = 0;
            speedCount = 0;
            tankCount = 0;
            bossCount = 1 * spawnMultiplier;
        }
        else if (x % 10 >= 6 && x % 10 <= 9)
        {
            speedCount = (x * 2 + 5) * spawnMultiplier;
            normalCount = speedCount;
            tankCount = Mathf.Max(speedCount - 1, 1);
            bossCount = 0;
        }
        
        else if (x % 10 >= 1 && x % 10 <= 5)
        {
            normalCount = ((x * 4 + 10) / 2) * spawnMultiplier;
            speedCount = ((x * 2) + 5) * spawnMultiplier;
            tankCount = ((x / 2) + 2) * spawnMultiplier;

            bossCount = 0;
        }
    }



    private IEnumerator SpawnWave()
    {
        isSpawning = true;

        WaveManagement();

        Debug.Log($"Wave {currentWave} will start spawning in {waveStartDelay} seconds...");
        yield return new WaitForSeconds(waveStartDelay);

        yield return SpawnGroup(normalMonster, normalCount);
        yield return SpawnGroup(speedMonster, speedCount);
        yield return SpawnGroup(tankMonster, tankCount);
        yield return SpawnGroup(bossMonster, bossCount);

        isSpawning = false;
    }

    private IEnumerator SpawnGroup(GameObject enemyPrefab, int count)
    {
        if (enemyPrefab == null || count <= 0) yield break;

        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPos = GetValidSpawnPosition();
            GameObject spawned = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            spawnedEnemies.Add(spawned);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private Vector2 GetValidSpawnPosition()
    {
        Vector2 spawnPos;
        int safetyCounter = 0;

        do
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * Random.Range(minSpawnDistance, maxSpawnDistance);
            spawnPos = (Vector2)player.position + randomOffset;
            safetyCounter++;
        }
        while (Vector2.Distance(spawnPos, player.position) < minSpawnDistance && safetyCounter < 50);

        return spawnPos;
    }
}
