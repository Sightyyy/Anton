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

    [Header("Wave Settings")]
    public int currentWave = 1;
    public int enemiesPerWave = 10;
    private int enemiesKilledThisWave = 0;

    [Header("Limits")]
    private const int maxActiveEnemies = 150;

    [Header("UI")]
    public TMP_Text waveText;
    public TMP_Text enemiesRemainingText;

    private float spawnTimer;
    private int activeEnemies = 0;
    private bool spawningEnabled = true;

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (!spawningEnabled) return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= timeBetweenSpawns &&
            activeEnemies < maxActiveEnemies &&
            (activeEnemies + enemiesKilledThisWave) < enemiesPerWave)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        // safety checks
        if (player == null)
        {
            Debug.LogError("EnemySpawner: Player belum di-assign!");
            return;
        }

        if ((IsBossWave() && bossPrefabs.Length == 0) || (!IsBossWave() && enemyPrefabs.Length == 0))
        {
            Debug.LogError("EnemySpawner: Prefab tidak tersedia untuk tipe wave ini.");
            return;
        }

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

        // attach event kalau ada
        EnemyBehavior eb = enemy.GetComponent<EnemyBehavior>();
        if (eb != null)
        {
            eb.onDeath += OnEnemyKilled;
        }

        activeEnemies++;
        UpdateUI();
    }

    private bool IsBossWave()
    {
        return currentWave % 10 == 0;
    }

    private void OnEnemyKilled()
    {
        activeEnemies = Mathf.Max(0, activeEnemies - 1);
        enemiesKilledThisWave++;
        UpdateUI();

        if (enemiesKilledThisWave >= enemiesPerWave)
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
