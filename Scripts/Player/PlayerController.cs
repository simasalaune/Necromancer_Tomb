using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject attackPrefab;
    public GameObject specialAttack;
    public Animator animAttack;
    public ParticleSystem dust;

    private GameObject tip;
    private Transform pivot;
    private MovementController mc;
    private float attackCD = 0.0f;
    private Animator anim;
    private SpriteRenderer sr;
    private PauseMenu pm;
    private Color damagedColor;
    private Color normalColor;
    private Color healedColor;
    private Color healed2Color;
    private Color restoredColor;
    private Coroutine coroutine;
    private Coroutine uiCoroutine;
    private Coroutine manaCoroutine;
    private Coroutine strongCoroutine;

    public bool gameOver = false;
    public int hitCount = 0;

    // Will be changable depending on character choice
    [HideInInspector]
    public float health;
    [HideInInspector]
    public float mana;
    [HideInInspector]
    public float maxHealth;
    [HideInInspector]
    public float maxMana;
    [HideInInspector]
    public int coins;

    public float damage;
    [HideInInspector]
    public float attackDuration;
    [HideInInspector]
    public float specialDuration;
    [HideInInspector]
    public float attackSpeed;
    [HideInInspector]
    public Vector3 attackOffset;
    [HideInInspector]
    public float knockback;
    [HideInInspector]
    public string Role;
    [HideInInspector]
    public HealthController healthController;
    [HideInInspector]
    public ManaController manaController;
    [HideInInspector]
    public CoinsController coinsController;
    [HideInInspector]
    public BackpackController backpackController;
    [HideInInspector]
    public bool inShop;
    [HideInInspector]
    public bool hasSpecial;
    [HideInInspector]
    public GameObject weapon;

    void Start()
    {
        // Setting variables
        mc = gameObject.GetComponent<MovementController>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        pm = FindObjectOfType<PauseMenu>();
        tip = GameObject.FindGameObjectWithTag("Tip");
        hasSpecial = false;
        normalColor = new Color(1.0f, 1.0f, 1.0f);
        damagedColor = new Color(0.83f, 0.54f, 0.54f);
        healedColor = new Color(0.41f, 0.69f, 0.35f);
        healed2Color = new Color(1, 0.36f, 0.36f);
        restoredColor = new Color(0.33f, 0.5f, 0.99f);
        weapon = GameObject.FindGameObjectWithTag("Weapon");
        pivot = weapon.transform.parent;
        anim = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if (pm.GameIsPaused || gameOver || mc.isStunned)
        {
            return;
        }


        if (!backpackController.inBackpack && !inShop)
        {
            // Get mouse position
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Make player`s weapon to rotate around player
            WeaponRotate(mousePosition);

            // Detecting player`s mouse clicks (attacks)
            if (Input.GetMouseButtonDown(0))
            {
                if (Time.time > attackCD)
                {
                    Attack(mousePosition);
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (Time.time > attackCD)
                {
                    StartCoroutine(SpecialAttack());
                    if (mana == maxMana)
                    {
                        //StartCoroutine(SpecialAttack());
                    }
                    else
                    {
                        if (manaCoroutine != null)
                        {
                            StopCoroutine(manaCoroutine);
                        }
                        manaCoroutine = StartCoroutine(manaController.NotEnoughMana());
                    }
                }
            }
        }

        // Quick healing
        if (Input.GetKeyDown(KeyCode.H))
        {
            int n = backpackController.SearchItem("Health potion");
            if (n > 0)
            {
                backpackController.RemoveItem("Health potion", 1);
                OnPlayerHealed(10.0f);
            }
        }

        if (health <= 0 && !gameOver)
        {
            gameOver = true;
            StartCoroutine(GameOver());
        }
    }

    // On collision with dropped items
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Drop_Coins")
        {
            if (backpackController.IsFull())
            {
                tip.SetActive(true);
                tip.GetComponent<TipController>().SetTip("Your backpack is full!", 10);
            }
            else
            {
                int count = int.Parse(collision.gameObject.name) + coins;
                SetCoins(count);
                Destroy(collision.gameObject);
            }
        }
        else if (collision.gameObject.tag == "Drop_Shroom")
        {
            if (backpackController.IsFull())
            {
                tip.SetActive(true);
                tip.GetComponent<TipController>().SetTip("Your backpack is full!", 10);
            }
            else
            {
                GameObject item = collision.gameObject;
                Sprite sprite = item.GetComponent<SpriteRenderer>().sprite;
                backpackController.AddItem(sprite, "JellyShroom");
                Destroy(collision.gameObject);
            }
        }
        else if (collision.gameObject.tag == "Drop_Stinger")
        {
            if (backpackController.IsFull())
            {
                tip.SetActive(true);
                tip.GetComponent<TipController>().SetTip("Your backpack is full!", 10);
            }
            else
            {
                GameObject item = collision.gameObject;
                Sprite sprite = item.GetComponent<SpriteRenderer>().sprite;
                backpackController.AddItem(sprite, "Stinger");
                Destroy(collision.gameObject);
            }
        }
        else if (collision.gameObject.tag == "Drop_Crystal")
        {
            if (backpackController.IsFull())
            {
                tip.SetActive(true);
                tip.GetComponent<TipController>().SetTip("Your backpack is full!", 10);
            }
            else
            {
                GameObject item = collision.gameObject;
                Sprite sprite = item.GetComponent<SpriteRenderer>().sprite;
                backpackController.AddItem(sprite, "Crystal");
                Destroy(collision.gameObject);
            }
        }
    }

    public void SetCoins(int count)
    {
        coins = count;
        coinsController.SetCoins(coins);
    }

    // Method called when player is damaged
    public void OnPlayerDamaged(float dmg, Vector2 direction, float knockback)
    {
        health -= dmg;
        if (health < 0)
        {
            health = 0;
        }

        healthController.SetHealth(health);
        mc.Knockback(direction, knockback);

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        if (uiCoroutine != null)
        {
            StopCoroutine(uiCoroutine);
        }

        coroutine = StartCoroutine(Fading(damagedColor));
        uiCoroutine = StartCoroutine(healthController.Fading(damagedColor, false));
    }

    public void OnPlayerHealed(float heal)
    {
        health += heal;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        healthController.SetHealth(health);

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        if (uiCoroutine != null)
        {
            StopCoroutine(uiCoroutine);
        }

        coroutine = StartCoroutine(Fading(healedColor));
        uiCoroutine = StartCoroutine(healthController.Fading(healed2Color, true));
    }

    public void OnPlayerRestoredMana(bool full)
    {
        if (full)
        {
            mana = maxMana;
            manaController.SetMana(mana);
        }

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(Fading(restoredColor));
    }

    public void OnPlayerSlowed(float factor, float duration)
    {
        StartCoroutine(mc.Slow(factor, duration));
    }

    public void OnPlayerStunned(float duration)
    {
        StartCoroutine(mc.Stun(duration));
    }

    public void OnPlayerConfused(float duration)
    {
        StartCoroutine(mc.Confuse(duration));
    }

    // Method for player`s basic attack
    void Attack(Vector3 mousePosition)
    {
        if (Role == "Knight" || Role == "Archer" || Role == "Wizard")
        {
            animAttack.SetTrigger("Attack");
        }

        if (Role == "Wizard" || Role == "Archer")
        {
            Vector3 attackSpawn = weapon.transform.TransformPoint(weapon.transform.localPosition + attackOffset);
            Quaternion rotation = CalculateRotation(attackSpawn, mousePosition);
            GameObject attack = Instantiate(attackPrefab, attackSpawn, rotation);           
        }
        attackCD = Time.time + attackDuration;
    }

    // Method for player`s special attack
    IEnumerator SpecialAttack()
    {
        if (mc == null)
        {
            yield break;
        }
        mana = 0;
        manaController.SetMana(mana);
        StartCoroutine(RestoreMana());

        weapon.transform.parent.gameObject.SetActive(false);
        StartCoroutine(mc.ResetForces(0));
        anim.SetTrigger("SpecialAttack");
        attackCD = Time.time + specialDuration;

        if (Role == "Archer")
        {
            StartCoroutine(mc.Dash());
            dust.Play();
        }
        else
        {
            mc.enabled = false;
        }
        
        yield return new WaitForSeconds(specialDuration);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Role == "Knight")
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 3.0f);
            foreach (Collider2D collider in hitColliders)
            {
                if (collider.tag == "Enemy")
                {
                    Vector2 direction = (collider.transform.position - transform.position).normalized;
                    EnemyController ec = collider.GetComponent<EnemyController>();
                    ec.OnEnemyAttacked(damage * 3, direction, knockback * 2);
                    if (ec.role != "Boss")
                    {
                        ec.OnEnemyStunned(3);
                    } 
                }
            }

            yield return new WaitForSeconds(specialDuration * 0.75f);
        }
        else if (Role == "Wizard")
        {
            Vector2 spawnPosition = new Vector2(transform.position.x, transform.position.y + 1.5f);
            Quaternion rotation = CalculateRotation(spawnPosition, mousePosition);
            GameObject attack = Instantiate(specialAttack, spawnPosition, rotation);
        }
        else if (Role == "Archer")
        {
            weapon.transform.parent.gameObject.SetActive(true);
            animAttack.SetTrigger("Attack");
            Vector3 attackSpawn = weapon.transform.TransformPoint(weapon.transform.localPosition + attackOffset);
            float mainAngle = CalculateAngle(attackSpawn, mousePosition);
            float angle = -30;
            while (angle <= 30)
            {
                GameObject current = Instantiate(specialAttack, attackSpawn, Quaternion.Euler(0, 0, mainAngle + angle));
                current.transform.localScale -= new Vector3(0.25f, 0.25f, 0);
                angle += 15;
            }
        }
        
        if (mc != null)
        {
            weapon.transform.parent.gameObject.SetActive(true);
            mc.enabled = true;
        } 
    }

    // Method to rotate weapon around player`s character
    void WeaponRotate(Vector3 mousePosition)
    {
        bool inverse = !mc.facingRight;
        float angle = CalculateAngle(pivot.position, mousePosition);
        Quaternion rot, wrot;
        if (inverse)
        {
            rot = Quaternion.Euler(0, 0, angle + 180.0f);
            wrot = Quaternion.Euler(0, 0, -(angle + 180.0f));
        }
        else
        {
            rot = Quaternion.Euler(0, 0, angle);
            wrot = Quaternion.Euler(0, 0, angle - 360.0f);
        }

        pivot.rotation = rot;
        if (Role == "Wizard")
        {
            weapon.transform.localRotation = wrot;
        }
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

    // Method for smooth fading in color
    public IEnumerator Fading(Color color)
    {
        sr.color = color;
        for (int i = 0; i < 20; i++)
        {
            sr.color = new Color(sr.color.r + 0.02f, sr.color.g + 0.02f, sr.color.b + 0.02f);
            yield return new WaitForSeconds(0.05f);
        }
        sr.color = normalColor;
    }

    // Method to restore mana over time
    public IEnumerator RestoreMana()
    {
        while (mana < maxMana)
        {
            mana++;
            manaController.SetMana(mana);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator GameOver()
    {
        Destroy(gameObject.GetComponent<MovementController>());
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        Destroy(gameObject.GetComponent<CapsuleCollider2D>());

        // Settings
        weapon.transform.parent.gameObject.SetActive(false);
        GameObject.FindGameObjectWithTag("Canvas").transform.Find("UI").gameObject.SetActive(false);

        // Animation
        anim.SetTrigger("Death");

        // Camera control
        Camera cam = Camera.main;
        float targetSize = cam.orthographicSize * 0.5f;
        while (cam.orthographicSize - targetSize > 0.1f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * 5);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject.GetComponent<SpriteRenderer>());

        /// DEFENCE
        /// 
        int highscore = PlayerPrefs.GetInt("hits");
        if (hitCount > highscore)
        {
            PlayerPrefs.SetInt("hits", hitCount);
            PlayerPrefs.Save();
        }

        pm.Death(hitCount, PlayerPrefs.GetInt("hits"));
        Debug.Log("Game Over!");
        Destroy(this);
    }
}
