using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    [SerializeField] float jumpForce = 1f;
    [SerializeField] float movementSpeed = 1f;

    public bool isGrounded = false;
    public bool isDead = false;
    public float movementDirection = 0f;

    float jumpTimer = 0f;

    public Vector3 respawnPoint;

    Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (jumpTimer > 0f)
            jumpTimer -= Time.deltaTime;
    }

    public void Move(float inputAxis)
    {
        if (!isDead)
            transform.position += new Vector3(inputAxis * movementSpeed * Time.deltaTime, 0, 0);
    }

    public void Jump()
    {
        if (isGrounded && !isDead && jumpTimer <= 0f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpTimer = 0.2f;
        }
    }

    public void Die()
    {
        isDead = true;
        transform.position = respawnPoint;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 0 || collision.gameObject.layer == 8)
        {
            if (transform.position.y >= collision.transform.position.y + collision.transform.lossyScale.y / 2f && !isGrounded)
                isGrounded = true;
            else if (transform.position.y < collision.transform.position.y + collision.transform.lossyScale.y / 2f && isGrounded)
                isGrounded = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((collision.gameObject.layer == 0 || collision.gameObject.layer == 8) && 
            transform.position.y > collision.transform.position.y + collision.transform.lossyScale.y / 2f && isGrounded)
        {
            isGrounded = false;
        }
    }
}
