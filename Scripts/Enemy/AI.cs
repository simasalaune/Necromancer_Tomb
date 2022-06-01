using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AI : MonoBehaviour
{
    public bool onCharge = false;
    public GameObject darkWizardAttack;
    public GameObject[] bossAttacks;
    public GameObject skeletonPrefab;
    public float knockbackCD = 0.0f;

    private float attackRange = 15f;
    private float speed = 3.5f;
    private bool facingRight = false;
    private float attackCD = 0.0f;
    private float checkCD = 0.0f;
    private float knockbackDuration = 0.5f;
    private bool isStunned = false;
    private Animator anim;
    private Animator animDebuff;
    private Rigidbody2D rb;
    private Tilemap groundTilemap;
    private Vector3 pos;
    private bool onAttack = false;
    
    private string role;
    private Coroutine aggroCoroutine;

    void Start()
    {
        role = gameObject.GetComponent<EnemyController>().role;
        anim = gameObject.GetComponent<Animator>();
        animDebuff = gameObject.transform.GetChild(0).GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        groundTilemap = GameObject.FindGameObjectWithTag("GroundTilemap").GetComponent<Tilemap>();   
    }

    void FixedUpdate()
    {
        // Enemy AI
        if (Time.time > knockbackCD && !isStunned)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            
            if (player == null)
            {
                return;
            }

            pos = player.transform.position;
            if (Vector3.Distance(transform.position, pos) < attackRange && player.GetComponent<PlayerController>() != null)
            {          
                // Attack stage       
                if (role == "Slime")
                {
                    anim.SetBool("isRunning", true);
                    Vector2 target = Vector2.MoveTowards(transform.position, pos, Time.fixedDeltaTime * speed);
                    rb.MovePosition(target);

                    if (aggroCoroutine == null)
                    {
                        aggroCoroutine = StartCoroutine(AggroEnemy());
                    }
                }
                else if (role == "Fly")
                {
                    if (!onAttack)
                    {
                        onAttack = true;
                        StartCoroutine(FlyAttack());
                        aggroCoroutine = StartCoroutine(AggroEnemy());
                    }
                }
                else if (role == "DarkWizard")
                {
                    if (!onAttack)
                    {
                        onAttack = true;
                        aggroCoroutine = StartCoroutine(AggroEnemy());
                        StartCoroutine(WizardAttack());
                    }
                }
                else if (role == "Boss")
                {
                    if (!onAttack)
                    {
                        onAttack = true;
                        aggroCoroutine = StartCoroutine(AggroEnemy());
                        StartCoroutine(BossAttack());
                    }
                }
                else if (role == "Skeleton")
                {
                    if (Vector3.Distance(transform.position, pos) > 2)
                    {
                        anim.SetBool("isRunning", true);
                        Vector2 target = Vector2.MoveTowards(transform.position, pos, Time.fixedDeltaTime * speed);
                        rb.MovePosition(target);
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                        anim.SetTrigger("Attack");
                        knockbackCD = Time.time + 0.75f;
                    }
                }

                if (!onCharge)
                {
                    if (pos.x > transform.position.x && !facingRight)
                    {
                        FlipSprite();
                    }
                    else if (pos.x < transform.position.x && facingRight)
                    {
                        FlipSprite();
                    }
                }
            }
            else
            {
                if (role == "Slime")
                {
                    anim.SetBool("isRunning", false);
                }
                aggroCoroutine = null;
            }
        }
    }

    // Dark wizard attack
    IEnumerator WizardAttack()
    {
        for (int i = 0; i < 3; i ++)
        {
            anim.SetTrigger("Attack");
            Vector3 attackSpawn = transform.position;
            Quaternion rotation = CalculateRotation(attackSpawn, pos);
            GameObject attack = Instantiate(darkWizardAttack, attackSpawn, rotation);
            yield return new WaitForSeconds(1);
        }
        // Teleport
        bad:
        Vector3 offset = Random.insideUnitCircle.normalized * Random.Range(3.0f, 6.0f);
        Vector3 point = transform.position + offset;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(point, 1.25f);

        if (!groundTilemap.HasTile(Vector3Int.FloorToInt(point)) || hitColliders.Length != 0)
        {
            goto bad;
        }
        yield return new WaitForSeconds(1);

        transform.position = point;
        onAttack = false;
    }

    // Dragonfly attack simulation
    IEnumerator FlyAttack()
    {
        // First stage
        int n = Random.Range(1, 4);
        while (n > 0)
        {
            bad:
            Vector3 offset = Random.insideUnitCircle.normalized * Random.Range(2.0f, 4.0f);
            Vector3 point = transform.position + offset;
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(point, 1.25f);
            
            if (!groundTilemap.HasTile(Vector3Int.FloorToInt(point)) || hitColliders.Length != 0)
            {
                goto bad;
            }

            anim.SetBool("isRunning", true);
            checkCD = Time.time + 1.0f;
            while (Vector2.Distance(transform.position, point) > 0.1f)
            {
                if (checkCD < Time.time)
                {
                    goto bad;
                }
                Vector2 target = Vector2.MoveTowards(transform.position, point, Time.fixedDeltaTime * speed * 2);
                rb.MovePosition(target);
                if (isStunned)
                {
                    yield return new WaitUntil(() => isStunned == false);
                }
                yield return new WaitForFixedUpdate();
            }
            rb.velocity = Vector3.zero;
            anim.SetBool("isRunning", false);
            n--;
            yield return new WaitForSeconds(0.5f);
        }

        // Second stage
        
        anim.SetBool("isCharging", true);
        animDebuff.SetBool("isAggroed", false);
        attackCD = Time.time + 1.0f;

        GameObject shadow = transform.GetChild(1).gameObject;
        shadow.SetActive(false);

        while (attackCD > Time.time)
        {
            float angle = CalculateAngle(transform.position, pos);
            bool inverse = !facingRight;
            if (inverse)
            {
                transform.rotation = Quaternion.Euler(0, 0, 180.0f + angle);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            yield return null;
        }

        if (isStunned)
        {
            yield return new WaitUntil(() => isStunned == false);
        }

        transform.GetComponent<EdgeCollider2D>().isTrigger = true;
        Vector2 destination = (pos - transform.position).normalized * 20.0f;
        rb.velocity = Vector3.zero;
        rb.AddForce(destination, ForceMode2D.Impulse);
        onCharge = true;
    }

    // Dragonfly attack end
    public IEnumerator FlyAttackEnd()
    {
        GameObject shadow = transform.GetChild(1).gameObject;
        shadow.SetActive(true);
        transform.rotation = Quaternion.identity;  
        anim.SetBool("isCharging", false);
        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(0.5f);

        onCharge = false;
        onAttack = false;
    }

    private IEnumerator BossAttack()
    {
        float projectileCD = 0;
        float projectileDuration = 2.0f;
        int attackCount = 15;
        Vector3 lastPos = transform.position;

        // First stage
        start:
        Vector3 offset = Random.insideUnitCircle.normalized * Random.Range(4.0f, 8.0f);
        Vector3 point = transform.position + offset;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(point, 2);

        if (!groundTilemap.HasTile(Vector3Int.FloorToInt(point)) || hitColliders.Length != 0)
        {
            goto start;
        }
        
        while (Vector2.Distance(transform.position, point) > 0.1f)
        {
            if (Time.time > projectileCD)
            {
                StartCoroutine(BossProjectile());
                projectileCD = Time.time + projectileDuration;
            }

            lastPos = transform.position;
            Vector2 target = Vector2.MoveTowards(transform.position, point, Time.fixedDeltaTime * speed);
            rb.MovePosition(target);
            yield return new WaitForFixedUpdate();
            if (lastPos == transform.position)
            {
                goto start;
            }
        }
        rb.velocity = Vector3.zero;
        
        EnemyController ec = gameObject.GetComponent<EnemyController>();

        // Enrage and second attack
        if (ec.health <= (ec.maxHealth / 2))
        {
            projectileDuration = 1.0f;
            speed = 4.5f;

            if (attackCount >= 15)
            {
                // Get spawn positions
                Vector3[] spawnPoints = new Vector3[3];
                for (int i = 0; i < 3; i ++)
                {
                    sklt:
                    Vector3 skeletonOffset = Random.insideUnitCircle.normalized * Random.Range(2.0f, 4.0f);
                    Vector3 skeletonPoint = transform.position + skeletonOffset;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(skeletonPoint, 1.25f);

                    if (!groundTilemap.HasTile(Vector3Int.FloorToInt(skeletonPoint)) || colliders.Length != 0)
                    {
                        goto sklt;
                    }

                    spawnPoints[i] = skeletonPoint;
                }

                // Spawn skeletons
                for (int i = 0; i < 3; i ++)
                {
                    anim.SetTrigger("Summon");
                    GameObject skeleton = Instantiate(skeletonPrefab, spawnPoints[i], Quaternion.identity);
                    skeleton.GetComponent<AI>().knockbackCD = Time.time + 1.5f;
                    
                    yield return new WaitForSeconds(1);
                }

                attackCount = 0;
                yield return new WaitForSeconds(1);
            }
            else
            {
                attackCount++;
            }
        }
        else
        {
            // Projectile speed changing according to boss hp
            projectileDuration = ec.health * 2 / ec.maxHealth;
        }

        goto start;
    }

    private IEnumerator BossProjectile()
    {
        anim.SetTrigger("Shoot");

        yield return new WaitForSeconds(0.5f);

        Vector3 attackSpawn = transform.position;
        Quaternion rotation = CalculateRotation(attackSpawn, pos);

        int i = Random.Range(0, 3);

        Instantiate(bossAttacks[i], attackSpawn, rotation);      
    }

    // Method for knockback simulation
    public void KnockBack(Vector2 direction, float knockback)
    {
        if (role != "Fly")
        {
            knockbackCD = Time.time + knockbackDuration;
        }

        if (!onCharge)
        {
            rb.AddForce(direction * knockback, ForceMode2D.Impulse);
            StartCoroutine(ResetForces());
        }
    }

    // Method to reset rigidbody`s forces
    IEnumerator ResetForces()
    {
        yield return new WaitForSeconds(knockbackDuration);
        if (!onCharge)
        {
            rb.velocity = Vector3.zero;
        }
    }

    public IEnumerator AggroEnemy()
    {
        animDebuff.SetBool("isAggroed", true);
        yield return new WaitForSeconds(1.0f);
        animDebuff.SetBool("isAggroed", false);
    }

    public IEnumerator StunEnemy(float duration)
    {
        animDebuff.SetBool("isStunned", true);
        anim.SetBool("isRunning", false);
        isStunned = true;
        yield return new WaitForSeconds(duration);
        animDebuff.SetBool("isStunned", false);
        animDebuff.SetBool("isAggroed", false);
        isStunned = false;
    }

    public IEnumerator SlowEnemy(float factor, float duration)
    {
        animDebuff.SetBool("isSlowed", true);
        float normalSpeed = speed;
        speed *= factor;
        yield return new WaitForSeconds(duration);
        speed = normalSpeed;
        animDebuff.SetBool("isSlowed", false);
    }

    // Method for flipping sprite by moving direction
    void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }

    // Calculate an angle between 2 points
    float CalculateAngle(Vector3 firstPosition, Vector3 secondPosition)
    {
        float angle = Mathf.Atan2(secondPosition.y - firstPosition.y, secondPosition.x - firstPosition.x);
        angle = angle / Mathf.PI * 180.0f;
        if (angle < 0)
        {
            angle += 360.0f;
        }
        return angle;
    }

    // Calculates Quaternion rotation 
    Quaternion CalculateRotation(Vector3 firstPosition, Vector3 secondPosition)
    {
        float angle = CalculateAngle(firstPosition, secondPosition);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        return rotation;
    }
    [Min(0f)]
    [SerializeField]
    private float moveSpeed = 2.5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        rb.velocity = new Vector3(horizontalInput * moveSpeed, rb.velocity.y, verticalInput * moveSpeed);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Cube"))
        {
            var force = other.gameObject.transform.localScale;
            rb.AddForce(Vector3.up * force.y * 10, ForceMode.Impulse);
            Debug.Log("Force = " + force.y * 20);
        }
    }
}
