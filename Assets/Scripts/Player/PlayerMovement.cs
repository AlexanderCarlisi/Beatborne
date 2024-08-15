using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;

    private PlayerInput playerInput;
    private Rigidbody rb;
    private Collider col;
    private Vector3 movementInput;
    private bool isGrounded;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }


    private void OnEnable()
    {
        playerInput.actions["Move"].started += OnMovementStarted;
        playerInput.actions["Move"].canceled += OnMovementCanceled;
        playerInput.actions["Jump"].performed += ctx => Jump();
    }
    private void OnDisable()
    {
        playerInput.actions["Move"].started -= OnMovementStarted;
        playerInput.actions["Move"].canceled -= OnMovementCanceled;
        playerInput.actions["Jump"].performed -= ctx => Jump();
    }


    private void OnMovementStarted(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector3>();
    }
    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        movementInput = Vector3.zero;
    }


    private void FixedUpdate()
    {
        Move();

    }


    private void Move()
    {
        Vector3 movement = movementInput * speed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y);
    }


    private void Jump()
    {
        if (isGrounded) rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
    }
}
