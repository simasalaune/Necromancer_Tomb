using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyController : MonoBehaviour
{
    public GameObject dropCoin;
    public GameObject dropShroom;
    public GameObject dropStinger;
    public GameObject dropCrystal;

    public float health;
    public float maxHealth;
    public float damage;
    public float knockback;
    public string role;

    private Color damagedColor;
    private Color normalColor;
    private SpriteRenderer sr;
    private AI ai;
    private Animator anim;
    private Animator animParticles;
    private Rigidbody2D rb;
    private bool dead = false;
    private PlayerController pc;

    void Start()
    {
        // Setting variables
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        normalColor = new Color(1.0f, 1.0f, 1.0f);
        damagedColor = new Color(0.83f, 0.54f, 0.54f);
        sr = transform.GetComponent<SpriteRenderer>();
        ai = transform.GetComponent<AI>();
        anim = gameObject.GetComponent<Animator>();
        animParticles = gameObject.transform.GetChild(0).GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (health <= 0.0f && !dead)
        {
            StartCoroutine(Death());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && !ai.onCharge)
        {
            Vector2 dir = (collision.gameObject.transform.position - transform.position).normalized;
            pc.OnPlayerDamaged(damage, dir, knockback);

            ai.KnockBack(Vector2.zero, 0);

            if (role == "Skeleton")
            {
                anim.SetTrigger("Attack");
            }

            if (role == "Boss")
            {
                pc.OnPlayerStunned(1);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (role == "Fly")
        {
            if (collision.gameObject.tag == "Player" && ai.onCharge)
            {
                Vector2 dir = rb.velocity.normalized;
                pc.OnPlayerDamaged(damage, dir, knockback);

                transform.GetComponent<EdgeCollider2D>().isTrigger = false;
                StartCoroutine(ai.FlyAttackEnd());
            }

            if (ai.onCharge && collision.gameObject.tag == "Wall")
            {
                transform.GetComponent<EdgeCollider2D>().isTrigger = false;
                StartCoroutine(ai.FlyAttackEnd());
            }  
        }
    }

    // Method called when enemy gets attacked
    public void OnEnemyAttacked(float dmg, Vector2 direction, float knockback)
    {
        /// DEFENCE
        pc.hitCount++;
        GameObject.FindGameObjectWithTag("HitCount").GetComponent<TextMeshProUGUI>().text = "Hits: " + pc.hitCount.ToString();



        health -= dmg;
        StartCoroutine(Damaged());

        // Add knockback
        ai.KnockBack(direction, knockback);
    }

    public void OnEnemyStunned(float duration)
    {
        StartCoroutine(ai.StunEnemy(duration));
    }

    public void OnEnemySlowed(float factor, float duration)
    {
        StartCoroutine(ai.SlowEnemy(factor, duration));
    }

    // Method for smooth fading in color
    IEnumerator Damaged()
    {
        sr.color = damagedColor;
        for (int i = 0; i < 20; i ++)
        {
            sr.color = new Color(sr.color.r + 0.01f, sr.color.g + 0.03f, sr.color.b + 0.03f);
            yield return new WaitForSeconds(0.05f);
        }
        sr.color = normalColor;
    }

    // Enemy drops something
    void Drops()
    {
        Vector2 pos = transform.position;
        
        if (role == "Slime")
        {
            GameObject coin = Instantiate(dropCoin, pos, Quaternion.identity);

            int n = Random.Range(0, 101);
            if (n > 80)
            {
                coin.name = "2";
            }
            else
            {
                coin.name = "1";
            }

            int m = Random.Range(0, 101);
            if (m > 80)
            {
                Instantiate(dropShroom, pos, Quaternion.identity);
            }
        }
        else if (role == "Fly")
        {
            GameObject coin = Instantiate(dropCoin, pos, Quaternion.identity);

            int n = Random.Range(0, 101);
            if (n > 80)
            {
                coin.name = "3";
            }
            else
            {
                coin.name = "1";
            }

            int m = Random.Range(0, 101);
            if (m > 80)
            {
                Instantiate(dropStinger, pos, Quaternion.identity);
            }
        }
        else if (role == "DarkWizard")
        {
            GameObject coin = Instantiate(dropCoin, pos, Quaternion.identity);

            int n = Random.Range(0, 101);
            if (n > 80)
            {
                coin.name = "4";
            }
            else
            {
                coin.name = "2";
            }

            int m = Random.Range(0, 101);
            if (m > 80)
            {
                Instantiate(dropCrystal, pos, Quaternion.identity);
            }
        }
    }

    // Enemy is dead!
    IEnumerator Death()
    {
        // Drops or/and animation
        dead = true;
        rb.velocity = Vector2.zero;
        Destroy(gameObject.GetComponent<AI>());
        Destroy(gameObject.GetComponent<CapsuleCollider2D>());
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        animParticles.SetBool("isSlowed", false);
        animParticles.SetBool("isStunned", false);
        animParticles.SetBool("isAggroed", false);
        anim.SetTrigger("Death");
        
        yield return new WaitForSeconds(0.7f);

        Drops();

        Destroy(gameObject);
    }

    [Min(0f)]
    [SerializeField]
    private float moveSpeed = 2.5f;

    [Min(0f)]
    [SerializeField]
    private float jumpForce = 2.5f;

    [SerializeField]
    private List<Material> materials;
    private int count = 0;
    private int max;

    [SerializeField]
    private GameObject ground;

    private Renderer currentMaterial;

    private float horizontalInput;
    private bool isJumping;
    private bool isGrounded;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentMaterial = ground.GetComponent<Renderer>();
        currentMaterial.enabled = true;
        currentMaterial.sharedMaterial = materials[count];
        max = materials.Count;
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
        rb.velocity = new Vector3(horizontalInput * moveSpeed, rb.velocity.y, 0);
        if (isJumping && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        isJumping = false;
    }

    private void ProcessInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump"))
            isJumping = true;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            currentMaterial.sharedMaterial = materials[count++];
            if (count >= max)
                count = 0;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
