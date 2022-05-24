using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    private PlayerController pc;

    void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            // Call OnAttack method from enemy script
            EnemyController ec = collision.GetComponent<EnemyController>();
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            ec.OnEnemyAttacked(pc.damage, direction, pc.knockback);
        }
    }
}
