using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnTime = 15.0f;
    public float range = 3.0f;

    private Vector3 spawn;
    private float spawnCD = 0.0f;

    void Start()
    {
        spawn = transform.position;
    }

    void Update()
    {
        if (Time.time > spawnCD)
        {
            Vector2 point = spawn;
            point += Random.insideUnitCircle * range;

            // Spawn only offscreen
            Vector3 view = Camera.main.WorldToViewportPoint(point);
            if (view.x < 0.0f || view.x > 1.0f || view.y < 0.0f || view.y > 1.0f)
            {
                Spawn(point);
            }
        }
    }

    // Method for spawning enemies
    void Spawn(Vector2 point)
    {
        GameObject enemy = Instantiate(enemyPrefab, point, Quaternion.identity);

        spawnCD = Time.time + spawnTime;
    }
}
