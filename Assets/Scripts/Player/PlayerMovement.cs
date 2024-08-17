using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _sprintMultiplier = 1.5f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _sneakMultiplier = 0.10f;

    private PlayerInput _playerInput;
    private InputAction _sprintAction;
    private InputAction _sneakAction;
    private InputAction _slideAction;

    private Rigidbody _rb;
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private Transform _model;

    public bool isWalking;
    public bool isGrounded;
    public bool isSprinting;
    public bool isSneaking;
    public bool isSliding;

    private bool _canJump { get { return isGrounded && !isSneaking && !isSliding; } }
    private bool _canSprint { get { return !isSneaking && isGrounded && isWalking; } }
    private bool _canSneak { get { return isGrounded && !isSprinting; } }
    private bool _canSlide { get { return isSprinting && isGrounded; } }

    private Vector2 _moveDirection;

    // Events for Listeners in other Scripts
    private bool _wasWalking;
    public readonly UnityEvent<bool> isWalkingEvent = new UnityEvent<bool>();
    private bool _wasSprinting;
    public readonly UnityEvent<bool> isSprintingEvent = new UnityEvent<bool>();
    private bool _wasSneaking;
    public readonly UnityEvent<bool> isSneakingEvent = new UnityEvent<bool>();
    // public readonly UnityEvent<bool> isSlidingEvent = new UnityEvent<bool>();
    public readonly UnityEvent<bool> isGroundedEvent = new UnityEvent<bool>();


    private void Awake() {
        _playerInput = GetComponent<PlayerInput>();
        _sprintAction = _playerInput.actions["Sprint"];
        _sneakAction = _playerInput.actions["Sneak"];
        _slideAction = _playerInput.actions["Slide"];

        _rb = GetComponent<Rigidbody>();
    }


    private void OnEnable() {
        _sprintAction.started += ctx => Sprint(true);
        _sprintAction.canceled += ctx => Sprint(false);

        _sneakAction.started += ctx => Sneak(true);
        _sneakAction.canceled += ctx => Sneak(false);

        // _slideAction.started += ctx => Slide(true);
        // _slideAction.canceled += ctx => Slide(false);
    }


    // Move InputAction
    private void OnMove(InputValue value)  {
        _moveDirection = value.Get<Vector2>();
    }
    // Jump InputAction
    private void OnJump(){
        if (_canJump) _rb.AddForce(new Vector3(0f, _jumpForce, 0f), ForceMode.Impulse);
    }


    private void Sprint(bool sprint) {
        if (sprint && _canSprint) isSprinting = true;
        else isSprinting = false;

        if (_wasSprinting != isSprinting) isSprintingEvent.Invoke(sprint);
        _wasSprinting = isSprinting;
    }
    private void Sneak(bool sneak) {
        if (sneak && _canSneak) isSneaking = true;
        else isSneaking = false;

        if (_wasSneaking != isSneaking) isSneakingEvent.Invoke(sneak);
        _wasSneaking = isSneaking;
    }
    // private void Slide(bool slide) {
    //     if (slide && _canSlide) isSliding = true;
    //     else isSliding = false;

    //     if (_canJump) isSlidingEvent.Invoke(slide);
    // }


    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            isGrounded = true;
            isGroundedEvent.Invoke(true);
        }
    }
    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            isGrounded = false;
            isGroundedEvent.Invoke(false);
        }
    }


    private void FixedUpdate() {
        Move();

        // if (isSliding) lower friction
        // if (!isSliding) reset friction
    }


    private void Move() {
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
        Vector3 velocity = worldDirection * _moveSpeed * GetMovementModifier();
        
        // Maintain the current vertical velocity (e.g., for jumping/falling)
        velocity.y = _rb.velocity.y;

        // Apply the new velocity to the Rigidbody
        _rb.velocity = velocity;

        // Rotate the player to face the direction of movement
        if (worldDirection != Vector3.zero) _model.rotation = Quaternion.LookRotation(worldDirection);

        // Update the isWalking flag
        isWalking = velocity.magnitude > 0;
        if (_wasWalking != isWalking) isWalkingEvent.Invoke(isWalking);
        _wasWalking = isWalking;
    }


    private float GetMovementModifier() {
        if (isSprinting) return _sprintMultiplier;
        else if (isSneaking) return _sneakMultiplier;
        return 1f;
    }
}
