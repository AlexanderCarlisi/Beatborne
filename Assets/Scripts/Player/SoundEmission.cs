using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundEmission : MonoBehaviour
{

    private class Emission {
        public float radius;
        public float volume;

        public Emission(float radius, float volume) {
            this.radius = radius;
            this.volume = volume;
        }
    }

    public float volume = 0f;

    private SphereCollider _soundTrigger;
    private PlayerMovement _playerMovementScript;

    private Emission _walkEmission = new Emission(7f, 5f);
    private Emission _sprintEmission = new Emission(8.5f, 5f); // always accumulates with Walk
    private Emission _sneakEmission = new Emission(-3f, -2f); // always accumulates with Walk
    private Emission _landingEmission = new Emission(13f, 20f);

    private float _burstEndTime = 0f;
    private Emission _burstEmission = new Emission(0, 0);
    private bool _duringBurstEmission { get { return Time.time < _burstEndTime; } }
    private bool _burstEnded = false;


    private void Awake() {
        _soundTrigger = GetComponent<SphereCollider>();
        _soundTrigger.isTrigger = true;

        _playerMovementScript = GetComponent<PlayerMovement>();
    }


    private void OnEnable() {
        _playerMovementScript.isWalkingEvent.AddListener(Walk);
        _playerMovementScript.isSprintingEvent.AddListener(Sprint);
        _playerMovementScript.isSneakingEvent.AddListener(Sneak);
        // _playerMovementScript.isSlidingEvent.AddListener(Slide);
        _playerMovementScript.isGroundedEvent.AddListener(Landing);
    }

    private void OnDisable() {
        _playerMovementScript.isWalkingEvent.RemoveListener(Walk);
        _playerMovementScript.isSprintingEvent.RemoveListener(Sprint);
        _playerMovementScript.isSneakingEvent.RemoveListener(Sneak);
        // _playerMovementScript.isSlidingEvent.RemoveListener(Slide);
        _playerMovementScript.isGroundedEvent.RemoveListener(Landing);
    }


    private void AddEmission(Emission emission) {
        _soundTrigger.radius += emission.radius;
        volume += emission.volume;

        // Debug.Log("Emission Add");
    }

    private void SubtractEmission(Emission emission) {
        // Debug.Log("Emission Subtract");
        if (_soundTrigger.radius - emission.radius < 0) {
            _soundTrigger.radius = 0;
            volume = 0;
            return;
        }
        _soundTrigger.radius -= emission.radius;
        volume -= emission.volume;
    }

    // // Need to be careful with this one, it can be abused.
    // private void SetEmission(Emission emission) {
    //     _soundTrigger.radius = emission.radius;
    //     volume = emission.volume;
    // }


    private void Walk(bool isWalking) {
        if (isWalking) AddEmission(_walkEmission);
        else SubtractEmission(_walkEmission);
    }

    private void Sprint(bool isSprinting) {
        if (isSprinting) AddEmission(_sprintEmission);
        else SubtractEmission(_sprintEmission);

        // Debug.Log("Sprinting: " + isSprinting);
    }

    private void Sneak(bool isSneaking) {
        if (isSneaking) AddEmission(_sneakEmission);
        else SubtractEmission(_sneakEmission);
    }

    // private void Slide(bool isSliding) {
    //     if (isSliding) AddEmission(_sprintEmission);
    //     else SubtractEmission(_sprintEmission);
    // }

    private void Landing(bool isGrounded) {
        if (isGrounded) BurstEmission(_landingEmission, 0.7f);
    }


    private void BurstEmission(Emission burst, float duration) {
        _burstEndTime = Time.time + duration;
        _burstEmission = burst;
        _burstEnded = false;

        AddEmission(burst);
    }


    private void FixedUpdate() {

        // Remove Burst Emission once it's over.
        if (!_duringBurstEmission && !_burstEnded) {
            SubtractEmission(_burstEmission);
            _burstEnded = true;
        }

    }
}