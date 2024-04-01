using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Vector3 cameraOffset = new(0, 3.5f, -5f);
    [SerializeField] private float cameraHorizontalSensitivity = 10f;
    [SerializeField] private float cameraVerticalSensitivity = 10f;
    [SerializeField] private Transform target;

    [SerializeField] private float verticalMinClamp = -30f;
    [SerializeField] private float verticalMaxClamp = 45f;

    private float _inputVerticalRotation = 0f;
    private float _cameraVerticalRotation = 0f;
    private float _targetDistance = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        transform.position = cameraOffset;
        transform.LookAt(GetCameraTargetPosition());
        _cameraVerticalRotation = transform.rotation.eulerAngles.x;
        _targetDistance = Vector3.Distance(transform.position, GetCameraTargetPosition());
    }

    private void Update()
    {
        // If we have vertical input, then move the camera
        if(_inputVerticalRotation != 0f)
            MoveCameraOnInput();
    }

    private void MoveCameraOnInput()
    {
        // Smooth input multiplying by sensitivity and deltaTime
        var smoothedVerticalInput = _inputVerticalRotation * cameraVerticalSensitivity * Time.deltaTime;

        // Rotate the camera
        _cameraVerticalRotation -= smoothedVerticalInput;
        _cameraVerticalRotation = Mathf.Clamp(_cameraVerticalRotation, verticalMinClamp, verticalMaxClamp);
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = _cameraVerticalRotation;
        transform.eulerAngles = targetRotation;

        // Then rotate the position along the Z axis
        transform.position = GetCameraTargetPosition() - (transform.forward * _targetDistance);
    }

    public float GetCameraHorizontalSensitivity()
    {
        return cameraHorizontalSensitivity;
    }

    public void SetVerticalRotation(float input)
    {
        _inputVerticalRotation = input;
    }

    private Vector3 GetCameraTargetPosition()
    {
        return target.position;
    }
}