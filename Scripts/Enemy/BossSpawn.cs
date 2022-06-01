using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawn : MonoBehaviour
{
    public GameObject boss;

    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }

        if (Vector2.Distance(player.transform.position, transform.position) < 10)
        {
            Instantiate(boss, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }
    public Transform goal;

    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;
    }

    [SerializeField]
    private List<GameObject> obstacles;

    [Min(0f)]
    [SerializeField]
    private int count = 6;

    [SerializeField]
    private Vector3 size = new Vector3(1f, 0f, 10f);


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, size);
    }
    private void Start()
    {
        CreateObstacles();
    }

    private void CreateObstacles()
    {
        for (var i = 0; i < count; i++)
        {
            foreach (var obstacle in obstacles)
            {
                Instantiate(obstacle, GetRandomPosition(),
                    obstacle.transform.rotation, gameObject.transform);
            }
        }
    }

    private Vector3 GetRandomPosition()
    {
        var volumePosition = new Vector3(
                Random.Range(0, size.x),
                Random.Range(0, size.y),
                Random.Range(0, size.z)
            );
        return transform.position + volumePosition - size / 2;
    }
}
