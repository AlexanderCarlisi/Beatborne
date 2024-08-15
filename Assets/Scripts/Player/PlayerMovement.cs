using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    private PlayerInput playerInput;
    // private InputAction moveAction;
    private InputAction jumpAction;

    private Rigidbody rb;
    private bool isGrounded;
    private Vector2 moveDirection;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        // moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];

        rb = GetComponent<Rigidbody>();
    }


    private void OnEnable()
    {
        jumpAction.performed += ctx => Jump();
    }
    private void OnDisable()
    {
        jumpAction.performed -= ctx => Jump();
    }


    // Move InputAction
    private void OnMove(InputValue value) {
        moveDirection = value.Get<Vector2>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
        }
    }


    private void FixedUpdate()
    {
        Move();
    }


    private void Move()
    {
        Vector3 velocity = new (moveDirection.x, 0f, moveDirection.y);
        velocity *= moveSpeed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
    }


    private void Jump()
    {
        if (isGrounded) rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
    }
}
