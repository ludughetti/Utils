using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Vector3 cameraOffset = new(0, 3.5f, -5f);
    [SerializeField] private float cameraInitialRotation = 15f;
    [SerializeField] private float cameraHorizontalSensitivity = 10f;
    [SerializeField] private float cameraVerticalSensitivity = 10f;
    [SerializeField] float verticalRotationClamp = 85f;
    [SerializeField] private float cameraElevation = 1f;
    [SerializeField] private Transform target;

    private float _inputVerticalRotation = 0f;
    private float _cameraVerticalRotation = 0f;
    private float _targetDistance = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        transform.position = cameraOffset;
        transform.eulerAngles = new Vector3(cameraInitialRotation, 0f);
        Debug.Log($"{name}: Euler angles {transform.rotation.eulerAngles}");
        _cameraVerticalRotation = transform.rotation.eulerAngles.x;
        _targetDistance = Vector3.Distance(transform.position, GetTargetElevatedPosition());
    }

    private void Update()
    {
        MoveCameraOnInput();
    }

    private void MoveCameraOnInput()
    {
        // First rotate
        _cameraVerticalRotation -= _inputVerticalRotation;
        _cameraVerticalRotation = Mathf.Clamp(_cameraVerticalRotation, -verticalRotationClamp, verticalRotationClamp);
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = _cameraVerticalRotation;
        transform.eulerAngles = targetRotation;

        // Then move
        transform.position = GetTargetElevatedPosition() - (transform.forward * _targetDistance);
    }

    public float GetCameraHorizontalSensitivity()
    {
        return cameraHorizontalSensitivity;
    }

    public void SetVerticalRotation(float input)
    {
        _inputVerticalRotation = input * cameraVerticalSensitivity * Time.deltaTime;
    }

    private Vector3 GetTargetElevatedPosition()
    {
        return target.position + new Vector3(0, cameraElevation);
    }
}
