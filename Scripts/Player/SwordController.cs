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

    [SerializeField]
    private float moveSpeed = 5.0f;

    [SerializeField]
    private Transform hand;

    [SerializeField]
    private GameObject gun1;

    private bool isHolding = false;

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

        if (isHolding)
        {
            gun1.transform.position = hand.position;
            Vector3 rotation = gun1.transform.rotation.eulerAngles;
            gun1.transform.rotation = Quaternion.Euler(rotation.x, transform.eulerAngles.y, rotation.z);
        }
        if (Input.GetKeyDown("q"))
        {
            isHolding = false;
            gun1.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Gun"))
        {
            if (!isHolding)
            {
                isHolding = true;
                gun1.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    [SerializeField]
    private float force = 10.0f;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float lifeTime;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
    }

    private void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab);
        Physics.IgnoreCollision(bullet.GetComponent<Collider>(),
                transform.GetComponent<Collider>());

        bullet.transform.position = transform.position;
        Vector3 rotation = bullet.transform.rotation.eulerAngles;
        bullet.transform.rotation = Quaternion.Euler(rotation.x, transform.eulerAngles.y, rotation.z);
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * speed, ForceMode.Impulse);

        StartCoroutine(DestroyBullet(bullet, lifeTime));
    }

    private IEnumerator DestroyBullet(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

    private float forcePunch = 0.0f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.forward * forcePunch, ForceMode.Impulse);
        }
        Destroy(gameObject);
    }

    private void Hit(float force)
    {
        forcePunch = force;
    }
    
    public GameObject player;
    [SerializeField]
    Vector3 cameraHeight = new Vector3(0f, 0f, 0f);

    void Update()
    {
        Vector3 pos = player.transform.position;
        pos += cameraHeight;
        transform.position = pos;
    }
}
