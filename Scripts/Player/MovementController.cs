using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [HideInInspector]
    public bool facingRight = false;
    [HideInInspector]
    public float speed;

    private Rigidbody2D rb;
    private Vector2 move;
    private Animator anim;
    private Animator animDebuff;

    private float knockbackCD = 0.0f;
    private bool dashing = false;

    [HideInInspector]
    public bool isStunned = false;

    [HideInInspector]
    public bool isConfused = false;

    void Start()
    {
        // Get player`s rigidbody
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        animDebuff = gameObject.transform.GetChild(1).GetComponent<Animator>();
    }

    void Update()
    {
        if (knockbackCD < Time.time && !dashing && !isStunned)
        {
            // Get movement variables from raw axis
            if (isConfused)
            {
                move.x = -Input.GetAxisRaw("Horizontal");
                move.y = -Input.GetAxisRaw("Vertical");
            }
            else
            {
                move.x = Input.GetAxisRaw("Horizontal");
                move.y = Input.GetAxisRaw("Vertical");
            }

            if (move.x == 0 && move.y == 0)
            {
                anim.SetBool("isRunning", false);
            }
            else
            {
                anim.SetBool("isRunning", true);
            }
        }
    }

    void FixedUpdate()
    {
        if (knockbackCD < Time.time && !dashing && !isStunned)
        {
            // Move player`s rigidbody
            Vector2 offset = move.normalized * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + offset);

            // Flip sprite by moving direction
            if (move.x > 0 && !facingRight)
            {
                FlipSprite();
            }
            else if (move.x < 0 && facingRight)
            {
                FlipSprite();
            }
        }
    }

    // Method for flipping players character by moving direction
    void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }

    public IEnumerator Dash()
    {
        dashing = true;
        if (move.x == 0 && move.y == 0)
        {
            if (facingRight)
            {
                rb.AddForce(Vector2.right * 10, ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(Vector2.left * 10, ForceMode2D.Impulse);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(move.normalized * 10, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(0.5f);

        rb.velocity = Vector2.zero;
        dashing = false;
    }

    public IEnumerator Slow(float factor, float duration)
    {
        animDebuff.SetBool("isSlowed", true);
        float normalSpeed = speed;
        speed *= factor;
        yield return new WaitForSeconds(duration);
        speed = normalSpeed;
        animDebuff.SetBool("isSlowed", false);
    }

    public IEnumerator Stun(float duration)
    {
        animDebuff.SetBool("isStunned", true);
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
        animDebuff.SetBool("isStunned", false);
    }

    public IEnumerator Confuse(float duration)
    {
        animDebuff.SetBool("isConfused", true);
        isConfused = true;
        yield return new WaitForSeconds(duration);
        isConfused = false;
        animDebuff.SetBool("isConfused", false);
    }

    public void Knockback(Vector2 direction, float knockback)
    {
        knockbackCD = Time.time + 0.25f;
        rb.AddForce(direction * knockback, ForceMode2D.Impulse);
        StartCoroutine(ResetForces(0.25f));
    }

    public IEnumerator ResetForces(float time)
    {
        if (time != 0)
        {
            yield return new WaitForSeconds(time);
        }
        rb.velocity = Vector3.zero;
    }
}
