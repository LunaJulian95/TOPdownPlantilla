using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float spawnTime = 3f;
    public int maxEnemies = 10;
    public float spawnRadius = 5f; // si no usas puntos, spawnea alrededor

    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnTime)
        {
            timer = 0;
            TrySpawn();
        }
    }

    void TrySpawn()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxEnemies) return;

        Vector3 pos;
        if (spawnPoints.Length > 0)
        {
            pos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }
        else
        {
            Vector2 rand = Random.insideUnitCircle * spawnRadius;
            pos = transform.position + new Vector3(rand.x, 0, rand.y);
        }

        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}