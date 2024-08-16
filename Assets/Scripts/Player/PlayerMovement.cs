using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _sprintMultiplier = 1.5f;
    [SerializeField] private float _jumpForce = 10f;

    private PlayerInput _playerInput;
    private InputAction _sprintAction;

    private Rigidbody _rb;
    [SerializeField] private Transform _cameraTarget;

    private bool _isGrounded;
    private Vector2 _moveDirection;


    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _sprintAction = _playerInput.actions["Sprint"];

        _rb = GetComponent<Rigidbody>();
    }


    private void OnEnable()
    {
        _sprintAction.started += ctx => { _moveSpeed *= _sprintMultiplier; };
        _sprintAction.canceled += ctx => { _moveSpeed /= _sprintMultiplier; };
    }


    // Move InputAction
    private void OnMove(InputValue value) {
        _moveDirection = value.Get<Vector2>();
    }
    // Jump InputAction
    private void OnJump()
    {
        if (_isGrounded) _rb.AddForce(new Vector3(0f, _jumpForce, 0f), ForceMode.Impulse);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _isGrounded = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _isGrounded = false;
        }
    }


    private void FixedUpdate()
    {
        Move();
    }


    private void Move()
    {
        // Convert the 2D move direction into a 3D world direction relative to the _cameraTarget
        Vector3 forward = _cameraTarget.forward;
        Vector3 right = _cameraTarget.right;

        // Normalize the directions to ensure consistent movement speed regardless of direction
        forward.y = 0f; // Keep the movement parallel to the ground
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Calculate the world-space movement direction
        Vector3 worldDirection = forward * _moveDirection.y + right * _moveDirection.x;
        Vector3 velocity = worldDirection * _moveSpeed;
        
        // Maintain the current vertical velocity (e.g., for jumping/falling)
        velocity.y = _rb.velocity.y;

        // Apply the new velocity to the Rigidbody
        _rb.velocity = velocity;
    }
}
