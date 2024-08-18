using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyBrain : MonoBehaviour
{
    // Sensitivity is the Update Rate.
    [SerializeField] private float _sensitivity = 0.1f;
    private float _nextTick = 0f;
    private float _currentSensitivity = 0f;

    [SerializeField] private float _alertness = 0.5f;

    [SerializeField] private float _directionThresholdVolume = 15f; // Detected Noise needs to accumulate to this value, within the given time threshold.
    [SerializeField] private float _directionThresholdTime = 1.5f;

    [SerializeField] private float _alertThresholdVolume = 25f; // Detected Noise needs to accumulate to this value, within the given time threshold.
    [SerializeField] private float _alertThresholdTime = 1f;
    
    [SerializeField] private float _accumulatedNoise = 0f;

    private Vector3 _targetDirection = Vector3.zero;
    [SerializeField] private GameObject _questionmark;
    [SerializeField] private GameObject _exclamationpoint;

    private Rigidbody _rb;
    [SerializeField] private float _moveSpeed = 5f;

    private bool _isAlerted = false;

    [SerializeField] private float directionThreshold;
    [SerializeField] private float alertThreshold;


    private void Awake() {
        _rb = GetComponent<Rigidbody>();
    }


    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            // Debug.Log("Battle Start");
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            Tick(other.gameObject);
        }
    }


    private void Tick(GameObject noiseSource) {
        if (Time.time < _nextTick) return;

        SoundEmission seScript = noiseSource.GetComponent<SoundEmission>();

        float currentNoise = seScript.volume;
        _accumulatedNoise += currentNoise * Time.deltaTime;

        // Linear interpolation to calculate thresholds
        directionThreshold = Mathf.Lerp(_directionThresholdVolume, _directionThresholdVolume * 0.5f, _accumulatedNoise / _directionThresholdTime);
        alertThreshold = Mathf.Lerp(_alertThresholdVolume, _alertThresholdVolume * 0.5f, _accumulatedNoise / _alertThresholdTime);

        _currentSensitivity = _sensitivity;
        if (_accumulatedNoise >= alertThreshold) {
            _exclamationpoint.SetActive(true);
            _questionmark.SetActive(false);
            _targetDirection = noiseSource.transform.position;
            _isAlerted = true;
            _currentSensitivity = 0.001f; // chase the player quicker.
        }
        else if (_accumulatedNoise >= directionThreshold) {
            _questionmark.SetActive(true);
            _exclamationpoint.SetActive(false);
            _targetDirection = noiseSource.transform.position;
            _isAlerted = false;

        } else {
            _isAlerted = false;
            _questionmark.SetActive(false);
            _exclamationpoint.SetActive(false);
            _targetDirection = Vector3.zero;
        }

        _nextTick = Time.time + _currentSensitivity;

        // Reset _accumulatedNoise if the thresholds are met or if no sound is detected
        if (currentNoise == 0) {
            _accumulatedNoise -= _alertness;
        }
    }


    private void MoveTowardsSound() {
        // Get the direction from the enemy to the sound position
        Vector3 directionToSound = (_targetDirection - transform.position).normalized;

        // Convert the direction to 3D world direction
        Vector3 forward = directionToSound;
        Vector3 right = Vector3.Cross(Vector3.up, forward); // Right vector perpendicular to the forward direction

        // Normalize the directions
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Calculate the world-space movement direction
        Vector3 worldDirection = forward;
        Vector3 velocity = _moveSpeed * (_isAlerted ? 2.5f : 1.0f) * worldDirection;

        // Maintain the current vertical velocity (e.g., for jumping/falling)
        velocity.y = _rb.velocity.y;

        // Apply the new velocity to the Rigidbody
        _rb.velocity = velocity;

        // Rotate the enemy to face the direction of the sound
        if (worldDirection != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(worldDirection);
        }
    }


    private void FixedUpdate() {
        MoveTowardsSound();
    }
}
