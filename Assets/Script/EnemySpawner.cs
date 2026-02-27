using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Settings")]
    // Changed to an array so you can drag in all 3 enemy types
    public GameObject[] enemyPrefabs;
    public Transform player;
    public float spawnInterval = 3f;
    public float spawnRadius = 10f;

    [Header("Optimization")]
    public int maxEnemies = 6; // Limit to 6 enemies

    [Header("Difficulty Scaling")]
    // How much the stats multiply every 10 seconds (0.1 means 10% stronger)
    public float difficultyIncreaseRate = 0.1f;
    private float _startTime;

    // We will count how many enemies are currently alive
    public static int currentEnemyCount = 0;

    IEnumerator Start()
    {
        currentEnemyCount = 0;
        _startTime = Time.time; // Record exactly when the game started

        // Wait 1 second for the MazeGenerator to finish building walls
        yield return new WaitForSeconds(1f);

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Only spawn if we are below the limit
            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }
        }
    }

    void SpawnEnemy()
    {
        if (player == null || enemyPrefabs.Length == 0) return;

        // 1. Pick a random enemy from your array (Warrior, Knight, or Wizard)
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject prefabToSpawn = enemyPrefabs[randomIndex];

        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 randomPoint = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas))
        {
            // 2. Spawn the enemy
            GameObject newEnemy = Instantiate(prefabToSpawn, hit.position, Quaternion.identity);

            // 3. Calculate difficulty multiplier based on time alive
            float timeAlive = Time.time - _startTime;
            // Example: After 60 seconds, multiplier is 1.0 + (60/10) * 0.1 = 1.6x stats
            float currentMultiplier = 1f + (timeAlive / 10f) * difficultyIncreaseRate;

            // 4. Apply the stats multiplier to the specific enemy
            EnemyAI aiScript = newEnemy.GetComponent<EnemyAI>();
            if (aiScript != null)
            {
                aiScript.ScaleDifficulty(currentMultiplier);
            }

            // Increase the count when an enemy is born
            currentEnemyCount++;
        }
    }
}