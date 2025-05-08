using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaveType
{
    Weak,
    Strong,
    Boss
}

[System.Serializable]
public class EnemyWave
{
    public WaveType waveType;
    public GameObject[] enemyPrefabs;
    public int[] enemyCounts;
    public float spawnInterval = 1f;

    [System.NonSerialized] public int[] originalCounts;

    public void Initialize()
    {
        originalCounts = new int[enemyCounts.Length];
        System.Array.Copy(enemyCounts, originalCounts, enemyCounts.Length);
    }

    public void ResetCounts()
    {
        if (originalCounts != null && originalCounts.Length == enemyCounts.Length)
        {
            System.Array.Copy(originalCounts, enemyCounts, enemyCounts.Length);
        }
    }
}

public class EnemyWaves : MonoBehaviour
{
    [Header("Wave Settings")]
    public List<EnemyWave> weakWaves;
    public List<EnemyWave> strongWaves;
    public List<EnemyWave> bossWaves;

    [Header("Spawn Points")]
    public List<Transform> spawnPoints;

    [Header("Spawn Effect")]
    public GameObject spawnEffectPrefab;

    private bool hasInitialSpawned = false;

    Combat combat;

    public int whatWaveIsIt = 1;

    private List<GameObject> activeEnemies = new List<GameObject>();

    private List<WaveType> waveTypePattern = new List<WaveType>
    {
        WaveType.Weak,
        WaveType.Weak,
        WaveType.Strong,
        WaveType.Weak,
        WaveType.Weak,
        WaveType.Strong,
        WaveType.Strong,
        WaveType.Strong,
        WaveType.Strong,
        WaveType.Boss
    };

    private int patternIndex = 0;
    private EnemyWave currentWave;

    private int enemiesRemainingToSpawn = 0;
    public int enemiesAlive = 0;

    private int spawnSubIndex = 0;
    private bool isSpawning = false;

    private float[] spawnTimers;

    void Start()
    {
        combat = GameObject.FindWithTag("Player").GetComponent<Combat>();
        if (spawnPoints.Count == 0)
        {
            Debug.Log("no point stupid");
            return;
        }

        spawnTimers = new float[spawnPoints.Count];
        for (int i = 0; i < spawnTimers.Length; i++)
        {
            spawnTimers[i] = 0f;
        }

        InitializeAllWaves(weakWaves);
        InitializeAllWaves(strongWaves);
        InitializeAllWaves(bossWaves);
    }

    void InitializeAllWaves(List<EnemyWave> waves)
    {
        foreach (var wave in waves)
        {
            wave.Initialize();
        }
    }

    void Update()
    {

        if (enemiesAlive < 0)
        {
            enemiesAlive = 0;
        }
        if (!isSpawning) return;


        if (!hasInitialSpawned)
        {
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                if (spawnSubIndex < currentWave.enemyPrefabs.Length && currentWave.enemyCounts[spawnSubIndex] > 0)
                {
                    SpawnEnemy(currentWave.enemyPrefabs[spawnSubIndex], spawnPoints[i]);
                    currentWave.enemyCounts[spawnSubIndex]--;
                    enemiesRemainingToSpawn--;

                    if (currentWave.enemyCounts[spawnSubIndex] <= 0)
                    {
                        spawnSubIndex++;
                    }
                }
            }
            hasInitialSpawned = true;
        }
        else
        {
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                spawnTimers[i] += Time.deltaTime;

                if (spawnSubIndex < currentWave.enemyPrefabs.Length && spawnTimers[i] >= currentWave.spawnInterval)
                {
                    if (currentWave.enemyCounts[spawnSubIndex] > 0)
                    {
                        SpawnEnemy(currentWave.enemyPrefabs[spawnSubIndex], spawnPoints[i]);
                        currentWave.enemyCounts[spawnSubIndex]--;
                        enemiesRemainingToSpawn--;

                        if (currentWave.enemyCounts[spawnSubIndex] <= 0)
                        {
                            spawnSubIndex++;
                        }
                    }

                    spawnTimers[i] = 0f;

                    if (enemiesRemainingToSpawn <= 0)
                    {
                        isSpawning = false;
                        currentWave.ResetCounts();
                        break;
                    }
                }
            }
        }



    }

    void SpawnEnemy(GameObject prefab, Transform spawnPoint)
    {
        GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        enemiesAlive++;
        activeEnemies.Add(enemy);

        if (spawnEffectPrefab != null)
        {
            GameObject effect = Instantiate(spawnEffectPrefab, spawnPoint.position, Quaternion.identity);
            Destroy(effect, 2f); // Fjern effekt efter 2 sekunder
        }
    }

    public void OnEnemyKilled(GameObject enemy)
    {
        enemiesAlive--;
        activeEnemies.Remove(enemy);

        if (enemiesAlive <= 0 && !isSpawning)
        {
            if (!combat.isControl) {
                GameObject.FindFirstObjectByType<ShopManager>().canOpenShop = true;
            }
            
            GameObject.FindGameObjectWithTag("Player").GetComponent<Combat>().playerHealth += 10f;
        }
    }

    public void StartNextWave()
    {
        if (patternIndex >= waveTypePattern.Count)
        {
            
            return;
        }

        WaveType currentType = waveTypePattern[patternIndex];
        patternIndex++;

        List<EnemyWave> pool = GetWavePool(currentType);
        if (pool == null || pool.Count == 0)
        {
            return;
        }

        currentWave = pool[Random.Range(0, pool.Count)];
        spawnSubIndex = 0;
        enemiesRemainingToSpawn = 0;
        hasInitialSpawned = false;

        whatWaveIsIt++;

        for (int i = 0; i < spawnTimers.Length; i++)
        {
            spawnTimers[i] = 0f;
        }

        for (int i = 0; i < currentWave.enemyCounts.Length; i++)
        {
            enemiesRemainingToSpawn += currentWave.enemyCounts[i];
        }

        isSpawning = true;
    }

    public void StopClearWave()
    {
        isSpawning = false;
        enemiesRemainingToSpawn = 0;
        hasInitialSpawned = false;
        patternIndex = 0;

        Test.amountWaves.Add(whatWaveIsIt);
        whatWaveIsIt = 1;
        currentWave.ResetCounts();


        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null) 
            {
                Destroy(enemy);
            }
        }
        activeEnemies.Clear(); 
        enemiesAlive = 0;
    }

    List<EnemyWave> GetWavePool(WaveType type)
    {
        switch (type)
        {
            case WaveType.Weak: return weakWaves;
            case WaveType.Strong: return strongWaves;
            case WaveType.Boss: return bossWaves;
            default: return null;
        }
    }
}
