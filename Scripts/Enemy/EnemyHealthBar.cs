using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{

    public Slider slider;
    public Slider sliderEffect;
    public Vector3 offset;

    private EnemyController hp;

    [SerializeField]
    private float hurtSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        hp = GetComponentInParent<EnemyController>();
        sliderEffect.value = hp.maxHealth;
        sliderEffect.maxValue = hp.maxHealth;
        slider.maxValue = hp.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        slider.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + offset);
        sliderEffect.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + offset);

        slider.value = hp.health;


        if (sliderEffect.value > slider.value)
            sliderEffect.value -= hurtSpeed;
        else
            sliderEffect.value = slider.value;
        
    }

    [Min(0f)]
    [SerializeField]
    private float moveSpeed = 2.5f;

    [Min(0f)]
    [SerializeField]
    private float jumpForce = 2.5f;

    private float horizontalInput;
    private float verticalInput;
    private bool isGrounded;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void ProcessInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }
    private void OnCollisionEnter(Collision other)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision other)
    {
        isGrounded = false;
    }
}
