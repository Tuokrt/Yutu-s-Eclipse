using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public float spawnRate = 2f;
    public float minHeight = -2f;
    public float maxHeight = 2f;

    private float nextSpawnTime;
     
    void Start()
    {
        nextSpawnTime = Time.time + spawnRate;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > nextSpawnTime)
        {
            SpawnObstacle();
            nextSpawnTime = Time.time+spawnRate;
        }
    }
    void SpawnObstacle()
    {
        float spawnHeight = Random.Range(minHeight, maxHeight);
        Vector3 spawnPosition = new Vector3(transform.position.x, spawnHeight, 0);
        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
    }
}
