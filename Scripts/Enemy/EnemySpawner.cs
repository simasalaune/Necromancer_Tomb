using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnTime = 15.0f;
    public float range = 3.0f;

    private Vector3 spawn;
    private float spawnCD = 0.0f;

    void Start()
    {
        spawn = transform.position;
    }

    void Update()
    {
        if (Time.time > spawnCD)
        {
            Vector2 point = spawn;
            point += Random.insideUnitCircle * range;

            // Spawn only offscreen
            Vector3 view = Camera.main.WorldToViewportPoint(point);
            if (view.x < 0.0f || view.x > 1.0f || view.y < 0.0f || view.y > 1.0f)
            {
                Spawn(point);
            }
        }
    }

    // Method for spawning enemies
    void Spawn(Vector2 point)
    {
        GameObject enemy = Instantiate(enemyPrefab, point, Quaternion.identity);

        spawnCD = Time.time + spawnTime;
    }

    [SerializeField]
    private float moveSpeed = 5.0f;

    [SerializeField]
    private Color color;

    [SerializeField]
    private TMP_Text text;

    private Color baseColor;
    private Renderer currentMaterial;
    private bool isCharging = false;
    private float T;
    private float force;
    private Hand hand;

    // Start is called before the first frame update
    void Start()
    {
        currentMaterial = GetComponent<Renderer>();
        baseColor = currentMaterial.materials[1].color;
        color.a = 0f;
        hand = GetComponentInChildren<Hand>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.LookAt(transform.position + direction);

        if (Input.GetKey(KeyCode.Space))
        {
            isCharging = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            hand.Punch(force);
            isCharging = false;
            currentMaterial.materials[1].color = baseColor;
            StartCoroutine(Show(force));
        }
        if (isCharging)
        {
            T += Time.deltaTime;
            force = T * 10f;
            color.a = T;
            currentMaterial.materials[1].color = color;
        }
        else
        {
            T = 0.0f;
            force = 0.0f;
        }
    }
    private IEnumerator Show(float force)
    {
        text.text = "Force = " + force;
        yield return new WaitForSeconds(3f);
        text.text = "Force = " + 0;
    }

    private Animator anim;
    private float forcePunch = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * forcePunch, ForceMode.Impulse);
        }
    }
    public void Punch(float force)
    {
        anim.SetTrigger("Punch");
        forcePunch = force;
    }
}
