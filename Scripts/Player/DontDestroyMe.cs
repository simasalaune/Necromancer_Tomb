using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyMe : MonoBehaviour
{
    // Start is called before the first frame update
    public bool CanBeDestroyed = false;

    [Min(0f)]
    [SerializeField]
    private float jumpForceMin = 1.0f;

    [Min(0f)]
    [SerializeField]
    private float jumpForceMax = 5.0f;

    [Min(0f)]
    [SerializeField]
    private float scaleMin = 1.0f;

    [Min(0f)]
    [SerializeField]
    private float scaleMax = 5.0f;

    private bool isGrounded = false;
    private bool canChange = false;

    private Rigidbody rb;

    [Header("Atsokimai")]
    [SerializeField]
    private int countMax = 0;
    private int count = 0;
    private bool canJump = true;
    private float force;
    private float scale;

    private Renderer color;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        color = GetComponent<Renderer>();
    }

    private void Start()
    {
        scale = Random.Range(scaleMin, scaleMax);
    }

    // Update is called once per frame
    void Update()
    {
        if (count >= countMax)
            canJump = false;
        if (canChange)
        {
            transform.localScale = new Vector3(scale, scale, scale);
            color.material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
        canChange = false;
        Debug.Log(count);
    }
    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        if (isGrounded && canJump)
        {
            force = Random.Range(jumpForceMin, jumpForceMax);
            scale = Random.Range(scaleMin, scaleMax);
            rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        count++;
        isGrounded = true;
        canChange = true;
    }

    private void OnCollisionExit(Collision other)
    {
        isGrounded = false;
    }

}
