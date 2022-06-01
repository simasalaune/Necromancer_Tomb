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

    [SerializeField]
    private float moveSpeed = 5.0f;

    [SerializeField]
    private Transform ball;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private Transform ballTarget;

    [SerializeField]
    private Transform hands;

    private bool isBallInHands = true;
    private bool isBallFlying = false;
    private float T;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.LookAt(transform.position + direction);

        if (isBallInHands)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                ball.position = target.position;

            }
            else
                ball.position = hands.position;

            if (Input.GetKeyUp(KeyCode.Space))
            {
                isBallInHands = false;
                isBallFlying = true;
                T = 0;
            }
        }
        if (isBallFlying)
        {
            T += Time.deltaTime;
            float duration = 0.5f;
            float T1 = T / duration;

            Vector3 position = Vector3.Lerp(target.position, ballTarget.position, T1);

            Vector3 arc = Vector3.up * 5 * Mathf.Sin(T1 * 3.14f);

            ball.position = position + arc;

            if (T1 >= 1)
            {
                isBallFlying = false;
                ball.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isBallInHands && !isBallFlying)
        {
            isBallInHands = true;
            ball.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private int score = 0;
    private int points = 0;
    [SerializeField]
    private TMP_Text scoreText;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private Transform ballTarget;

    private float distance;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        distance = (ballTarget.position.x - player.position.x) +
            (ballTarget.position.z - player.position.z);
        scoreText.text = "Score: " + score;
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            score += points;
        }
        if (collision.gameObject.CompareTag("Zone1"))
        {
            points = 30 + (int)distance;
        }
        if (collision.gameObject.CompareTag("Zone2"))
        {
            points = 20 + (int)distance;
        }
        if (collision.gameObject.CompareTag("Zone3"))
        {
            points = 10 + (int)distance;
        }
    }
}
