using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public bool isWizardSpecial = false;
    public bool isArcherSpecial = false;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private float speed;
    private float damage;
    private float knockback;
    private Animator anim;
    private bool explosion = false;
    private string role;
    private bool collided = false;
    private Vector2 colliderOffset;
    private Collider2D enemyCollider;

    void Start()
    {
        // Get needed variables from player`s controller`s script
        PlayerController pc = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<PlayerController>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = transform.GetComponent<SpriteRenderer>();

        // Setting variables
        speed = pc.attackSpeed;
        damage = pc.damage;
        knockback = pc.knockback;
        role = pc.Role;
        anim = gameObject.GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        // Move object towards player`s mouse click (it`s own facing direction)
        if (!explosion)
        {
            transform.position += transform.right * Time.deltaTime * speed;
        }
        else
        {
            // Dynamic arrow attaching
            if (role == "Archer" && enemyCollider != null)
            {
                Vector2 newOffset = enemyCollider.offset;
                if (colliderOffset != newOffset)
                {
                    transform.localPosition += new Vector3(newOffset.x - colliderOffset.x, newOffset.y - colliderOffset.y, 0);
                    colliderOffset = newOffset;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collided == false)
        {
            string t = collision.tag;
            if (t == "Enemy")
            {
                // Call OnAttack method from enemy script
                EnemyController ec = collision.GetComponent<EnemyController>();
                Vector2 direction = (collision.transform.position - transform.position).normalized;

                if (isArcherSpecial)
                {
                    ec.OnEnemyAttacked(damage * 3, direction, knockback);
                }
                else
                {
                    transform.SetParent(collision.transform);
                    sr.sortingOrder = 1;

                    if (role == "Archer" && ec.role == "Slime")
                    {
                        enemyCollider = collision.GetComponent<Collider2D>();
                        colliderOffset = enemyCollider.offset;
                        Destroy(transform.GetComponent<TrailRenderer>());
                    }

                    ec.OnEnemyAttacked(damage, direction, knockback);

                    StartCoroutine(Explode());
                    collided = true;
                }        
            }
            else if (t == "Wall")
            {
                StartCoroutine(Explode());
                collided = true;
            }   
        }
    }

    public IEnumerator Explode()
    {
        // Animating and destroying attack object
        explosion = true;
        if (role == "Wizard")
        {
            if (isWizardSpecial)
            {
                anim.SetTrigger("ExplosionTrigger");

                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 3.0f);
                foreach (Collider2D collider in hitColliders)
                {
                    if (collider.tag == "Enemy")
                    {
                        Vector2 direction = (collider.transform.position - transform.position).normalized;
                        EnemyController ec = collider.GetComponent<EnemyController>();
                        ec.OnEnemyAttacked(damage * 3, direction, knockback);
                        ec.OnEnemySlowed(0.5f, 3);
                    }
                }

                yield return new WaitForSeconds(1.0f);
            }
            else
            {
                anim.Play("Explosion");
                yield return new WaitForSeconds(0.4f);
            }
        }
        else if (role == "Archer")
        { 
            anim.SetTrigger("ExplosionTrigger");
            while (true)
            {
                if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Explosion")
                {
                    break;
                }
                yield return null;
            }
            Color alpha = sr.color;
            for (int i = 0; i < 20; i ++)
            {
                alpha.a -= 0.05f;
                sr.color = alpha;
                yield return new WaitForSeconds(0.02f);
            }
        }
        Destroy(gameObject);
    }

    [SerializeField]
    private float hp;

    [SerializeField]
    private float maxHp = 40.0f;

    private Renderer texture;
    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp;
        texture = GetComponent<Renderer>();
        color = texture.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0)
            Destroy(gameObject);
        texture.material.color = new Color(color.r, color.g, color.b, color.a);
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        color.a -= damage / 50;
    }

    [SerializeField]
    private float hp;

    [SerializeField]
    private float maxHp = 40.0f;

    private Renderer texture;
    private Color color;
    [SerializeField]
    private GameObject player;
    private NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp;
        texture = GetComponent<Renderer>();
        color = texture.material.color;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = player.transform.position;
        if (hp <= 0)
            Destroy(gameObject);
        texture.material.color = new Color(color.r, color.g, color.b, color.a);
    }

    public void TakeDamage1(float damage)
    {
        hp -= damage;
        color.a -= damage / 50;
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

    private HP hp;
    private HP_Enemy hpEnemy;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hp = GetComponent<HP>();
    }

    private void Start()
    {
        hpEnemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<HP_Enemy>();
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

        if (Input.GetKeyDown("q"))
            hpEnemy.TakeDamage1(10.0f);
    }
    private void OnCollisionEnter(Collision other)
    {
        isGrounded = true;
        hp.TakeDamage(10.0f);
    }

    private void OnCollisionExit(Collision other)
    {
        isGrounded = false;
    }
}
