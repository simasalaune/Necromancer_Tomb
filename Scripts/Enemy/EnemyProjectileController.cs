using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileController : MonoBehaviour
{
    public float speed;
    public float damage;
    public float knockback;
    public string role;

    private Animator anim;
    private bool collided = false;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!collided)
        {
            transform.position += transform.right * Time.deltaTime * speed;
        }  
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collided)
        {
            string t = collision.tag;
            if (t == "Player")
            {
                PlayerController pc = collision.GetComponent<PlayerController>();
                Vector2 direction = (collision.transform.position - transform.position).normalized;

                pc.OnPlayerDamaged(damage, direction, knockback);

                if (role == "DarkWizard")
                {
                    pc.OnPlayerSlowed(0.5f, 1);
                }
                else if (role == "Boss")
                {
                    if (gameObject.tag == "BossSlow")
                    {
                        pc.OnPlayerSlowed(0.5f, 1);
                    }
                    else if (gameObject.tag == "BossStun")
                    {
                        pc.OnPlayerStunned(1);
                    }
                    else if (gameObject.tag == "BossConfuse")
                    {
                        pc.OnPlayerConfused(1);
                    }
                }

                StartCoroutine(Explode());
                collided = true;
            }
            else if (t == "Wall")
            {
                StartCoroutine(Explode());
                collided = true;
            }
        }
    }

    IEnumerator Explode()
    {
        anim.SetTrigger("ExplosionTrigger");
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
