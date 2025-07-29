using UnityEngine;
using System.Collections.Generic;

public class DecorationSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject decorationPrefab;
    [SerializeField] private float immediateSpawnRadius = 5f;
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private float despawnRadius = 25f;
    [SerializeField] private int maxActiveDecorations = 50;
    [SerializeField] private float minDistanceBetween = 2f;

    [Header("Density Settings")]
    [SerializeField] private float checkInterval = 0.3f;
    [SerializeField] private int spawnAttemptsPerCheck = 5;
    [SerializeField] private int initialTreesToSpawn = 5;

    [Header("Organization")]
    [SerializeField] private string parentObjectName = "SpawnedDecorations";
    private Transform decorationsParent;

    private Transform playerTransform;
    private Queue<GameObject> decorationPool = new Queue<GameObject>();
    private List<GameObject> activeDecorations = new List<GameObject>();
    private List<Vector2> spawnedPositions = new List<Vector2>();
    private float nextCheckTime;
    private bool hasSpawnedInitialTrees = false;

    void Awake()
    {
        playerTransform = transform;
        CreateParentObject();
        InitializePool();
    }

    void CreateParentObject()
    {
        GameObject parentObj = GameObject.Find(parentObjectName);
        if (parentObj == null)
        {
            parentObj = new GameObject(parentObjectName);
        }
        decorationsParent = parentObj.transform;
    }

    void Start()
    {
        SpawnInitialTrees();
    }

    void Update()
    {
        if (Time.time >= nextCheckTime)
        {
            ManageDecorations();
            nextCheckTime = Time.time + checkInterval;
        }
    }

    void SpawnInitialTrees()
    {
        for (int i = 0; i < initialTreesToSpawn; i++)
        {
            Vector2 spawnPos = GetCirclePosition(immediateSpawnRadius, i, initialTreesToSpawn);
            if (IsPositionValid(spawnPos))
            {
                SpawnDecoration(spawnPos);
            }
        }
        hasSpawnedInitialTrees = true;
    }

    Vector2 GetCirclePosition(float radius, int index, int total)
    {
        float angle = index * (2f * Mathf.PI / total);
        return (Vector2)playerTransform.position + new Vector2(
            Mathf.Cos(angle) * radius,
            Mathf.Sin(angle) * radius
        );
    }

    void InitializePool()
    {
        for (int i = 0; i < maxActiveDecorations; i++)
        {
            GameObject deco = Instantiate(decorationPrefab, decorationsParent);
            deco.SetActive(false);
            decorationPool.Enqueue(deco);
        }
    }

    void ManageDecorations()
    {
        // Despawn distant decorations
        for (int i = activeDecorations.Count - 1; i >= 0; i--)
        {
            GameObject deco = activeDecorations[i];
            float distance = Vector2.Distance(playerTransform.position, deco.transform.position);

            if (distance > despawnRadius && (hasSpawnedInitialTrees || distance > immediateSpawnRadius * 1.5f))
            {
                DespawnDecoration(deco);
            }
        }

        // Spawn new decorations if needed
        if (activeDecorations.Count < maxActiveDecorations)
        {
            TrySpawnDecorations();
        }
    }

    void TrySpawnDecorations()
    {
        for (int i = 0; i < spawnAttemptsPerCheck; i++)
        {
            Vector2 spawnPosition = GetRandomSpawnPosition();

            if (IsPositionValid(spawnPosition))
            {
                SpawnDecoration(spawnPosition);
            }
        }
    }

    Vector2 GetRandomSpawnPosition()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(spawnRadius - 5f, spawnRadius);
        return (Vector2)playerTransform.position + new Vector2(
            Mathf.Cos(angle) * distance,
            Mathf.Sin(angle) * distance
        );
    }

    bool IsPositionValid(Vector2 position)
    {
        float minDistSqr = minDistanceBetween * minDistanceBetween;
        foreach (Vector2 existingPos in spawnedPositions)
        {
            if ((existingPos - position).sqrMagnitude < minDistSqr)
            {
                return false;
            }
        }
        return true;
    }

    void SpawnDecoration(Vector2 position)
    {
        if (decorationPool.Count == 0) return;

        GameObject deco = decorationPool.Dequeue();
        deco.transform.position = position;
        deco.transform.SetParent(decorationsParent); // Ensure proper parenting
        deco.SetActive(true);
        activeDecorations.Add(deco);
        spawnedPositions.Add(position);
    }

    void DespawnDecoration(GameObject deco)
    {
        deco.SetActive(false);
        activeDecorations.Remove(deco);
        spawnedPositions.Remove(deco.transform.position);
        decorationPool.Enqueue(deco);
    }
}