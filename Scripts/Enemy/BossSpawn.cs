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
}
