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

    [Min(0f)]
    [SerializeField]
    private float moveSpeed = 2.5f;

    [Min(0f)]
    [SerializeField]
    private float jumpForce = 2.5f;

    private float horizontalInput;
    private float verticalInput;
    private bool isJumping;
    private bool isGrounded;

    private Rigidbody rb;

    private GameObject[] cubes;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        cubes = GameObject.FindGameObjectsWithTag("Cube");
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }
    void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        rb.velocity = new Vector3(horizontalInput * moveSpeed, rb.velocity.y, verticalInput * moveSpeed);
        if (isJumping && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        isJumping = false;
    }

    private void ProcessInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
            isJumping = true;
    }
    private void OnCollisionEnter(Collision other)
    {
        isGrounded = true;
        foreach (GameObject cube in cubes)
        {
            cube.transform.localScale = new Vector3(cube.transform.localScale.x, Random.Range(0.1f, 3), cube.transform.localScale.z);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        isGrounded = false;
    }
}
