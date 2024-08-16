using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private Transform _followTarget;

    [SerializeField] private float _rotationalSpeed = 10f;
    [SerializeField] private float _topClamp = 70f;
    [SerializeField] private float _bottomClamp = -40f;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private InputAction _lookAction;


    private void Awake() {
        _lookAction = GetComponent<PlayerInput>().actions["Look"];
    }


    private void LateUpdate() { // Makes sure the camera is updated after the player has moved
        CameraLogic();
    }


    private void CameraLogic() {
        float mouseX = _lookAction.ReadValue<Vector2>().x * _rotationalSpeed * Time.deltaTime;;
        float mouseY = _lookAction.ReadValue<Vector2>().y * _rotationalSpeed * Time.deltaTime;;

        _cinemachineTargetPitch = UpdateRotation(_cinemachineTargetPitch, mouseY, _bottomClamp, _topClamp, true);
        _cinemachineTargetYaw = UpdateRotation(_cinemachineTargetYaw, mouseX, float.MinValue, float.MaxValue, false);

        ApplyRotation();
    }


    private void ApplyRotation() {
        _followTarget.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0);
    }


    private float UpdateRotation(float currentRotation, float input, float min, float max, bool isXAxis) {
        currentRotation += isXAxis ? -input : input;
        return Mathf.Clamp(currentRotation, min, max);
    }
}
