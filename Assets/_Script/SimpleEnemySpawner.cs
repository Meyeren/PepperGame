using UnityEngine;

public class SimpleEnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject[] enemyPrefabs;   
    public int numberToSpawn = 5;
    public float spawnInterval = 2f;
    public Vector3 spawnOffset = Vector3.zero;

    private int spawnedCount = 0;
    private float timer = 0f;

    [Header("Prefab Selection")]
    public int prefabIndex = 0;       

    void Update()
    {
        if (spawnedCount >= numberToSpawn) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0 || prefabIndex >= enemyPrefabs.Length)
        {
            Debug.LogWarning("No valid prefab to spawn.");
            return;
        }

        Vector3 spawnPosition = transform.position + spawnOffset;
        Instantiate(enemyPrefabs[prefabIndex], spawnPosition, Quaternion.identity);
        spawnedCount++;
    }

    public void SetPrefabIndex(int index)
    {
        if (index >= 0 && index < enemyPrefabs.Length)
        {
            prefabIndex = index;
        }
        else
        {
            Debug.LogWarning("Invalid prefab index.");
        }
    }
}
