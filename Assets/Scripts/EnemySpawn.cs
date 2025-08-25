using UnityEngine;
using TMPro;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs;
    public GameObject[] bossPrefabs;

    [Header("Spawn Distance Settings")]
    public Transform player;
    public float minSpawnDistance = 5f;
    public float maxSpawnDistance = 15f;

    [Header("Spawn Timing")]
    public float timeBetweenSpawns = 2f;
    public float waveDelay = 3f;
    public float timeBetweenGroupSpawns = 0.5f;

    [Header("Wave Settings")]
    public int currentWave = 1;
    public int maxWave;
    public int enemiesPerWave = 10;
    public int enemiesKilledThisWave = 0;
    private int enemiesSpawnedThisWave = 0;

    [Header("Group Spawning")]
    public int minGroupSize;
    public int maxGroupSize;

    [Header("Limits")]
    private const int maxActiveEnemies = 100;

    [Header("UI")]
    public TMP_Text waveText;
    public TMP_Text enemiesRemainingText;

    private float spawnTimer;
    private int activeEnemies = 0;
    private bool spawningEnabled = true;
    private bool isSpawningGroup = false;

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (!spawningEnabled || isSpawningGroup) return;

        spawnTimer += Time.deltaTime;

        if (currentWave == maxWave && enemiesSpawnedThisWave < enemiesPerWave)
        {
            SpawnBossOnly();
            spawnTimer = 0f;
            return;
        }

        if (spawnTimer >= timeBetweenSpawns &&
            activeEnemies < maxActiveEnemies &&
            enemiesSpawnedThisWave < enemiesPerWave)
        {
            StartCoroutine(SpawnEnemyGroup());
            spawnTimer = 0f;
        }
    }

    private IEnumerator SpawnEnemyGroup()
    {
        isSpawningGroup = true;
        int remainingEnemies = enemiesPerWave - enemiesSpawnedThisWave;

        int groupSize = Mathf.Min(
            Random.Range(minGroupSize, maxGroupSize + 1),
            remainingEnemies
        );

        if (remainingEnemies - groupSize > 0 &&
            remainingEnemies - groupSize < minGroupSize)
        {
            groupSize = remainingEnemies;
        }

        for (int i = 0; i < groupSize; i++)
        {
            if (activeEnemies >= maxActiveEnemies ||
                enemiesSpawnedThisWave >= enemiesPerWave)
            {
                break;
            }

            SpawnSingleEnemy();
            yield return new WaitForSeconds(timeBetweenGroupSpawns);
        }

        isSpawningGroup = false;
    }

    private void SpawnSingleEnemy()
    {
        Vector2 spawnPos;
        int safety = 0;
        do
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
            spawnPos = (Vector2)player.position + randomDirection * randomDistance;
            safety++;
        }
        while (Vector2.Distance(player.position, spawnPos) < minSpawnDistance && safety < 30);

        GameObject prefabToSpawn;
        if (IsBossWave())
            prefabToSpawn = bossPrefabs[Random.Range(0, bossPrefabs.Length)];
        else
            prefabToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        GameObject enemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        EnemyBehavior eb = enemy.GetComponent<EnemyBehavior>();
        if (eb != null)
        {
            eb.onDeath += OnEnemyKilled;
        }

        activeEnemies++;
        enemiesSpawnedThisWave++;
        UpdateUI();
    }

    private void SpawnBossOnly()
    {
        Vector2 spawnPos = (Vector2)player.position + Random.insideUnitCircle.normalized * Random.Range(minSpawnDistance, maxSpawnDistance);

        GameObject boss = Instantiate(bossPrefabs[Random.Range(0, bossPrefabs.Length)], spawnPos, Quaternion.identity);

        EnemyBehavior eb = boss.GetComponent<EnemyBehavior>();
        if (eb != null)
        {
            eb.onDeath += OnEnemyKilled;
        }

        activeEnemies = 1;
        enemiesSpawnedThisWave = 1;
        enemiesPerWave = 1;
        UpdateUI();

        spawningEnabled = false;
    }

    private bool IsBossWave()
    {
        return currentWave % maxWave == 0;
    }

    private void OnEnemyKilled()
    {
        activeEnemies = Mathf.Max(0, activeEnemies - 1);
        enemiesKilledThisWave++;
        UpdateUI();

        if (enemiesKilledThisWave >= enemiesPerWave && currentWave < maxWave)
        {
            StartCoroutine(StartNextWaveWithDelay());
        }
    }

    private IEnumerator StartNextWaveWithDelay()
    {
        spawningEnabled = false;

        yield return new WaitForSeconds(2f);

        currentWave++;
        enemiesKilledThisWave = 0;
        enemiesSpawnedThisWave = 0;

        if (IsBossWave())
            enemiesPerWave = 1;
        else
            enemiesPerWave += 5;

        UpdateUI();

        yield return new WaitForSeconds(waveDelay);

        spawnTimer = 0f;
        spawningEnabled = true;
    }

    private void UpdateUI()
    {
        if (waveText != null) waveText.text = $"Wave {currentWave}";
        if (enemiesRemainingText != null)
            enemiesRemainingText.text = $"Enemies Left: {Mathf.Max(enemiesPerWave - enemiesKilledThisWave, 0)}";
    }
}
