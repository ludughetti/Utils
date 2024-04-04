using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Vector3 cameraOffset = new(0, 3.5f, -5f);
    [SerializeField] private float cameraSensitivity = 10f;
    [SerializeField] private Transform target;
    [SerializeField] private CharacterBody characterBody;

    [SerializeField] private float verticalMinClamp = -30f;
    [SerializeField] private float verticalMaxClamp = 45f;

    private Vector2 _inputRotation = Vector2.zero;
    private Vector2 _rawInputRotation = Vector2.zero;
    private float _targetDistance = 0f;
    private float _rotX = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Offset camera for third person view
        transform.position = cameraOffset;
        transform.LookAt(target.position);

        // Calculate distance between positions for camera rotation and repositioning
        _targetDistance = Vector3.Distance(transform.position, target.position);
    }

    private void FixedUpdate()
    {
        RotateCameraOnBodyRotation();
        MoveCamera();
    }

    private void RotateCameraOnBodyRotation()
    {
        // If we don't have input, then skip rotation calculations
        if (Vector2.zero.Equals(_rawInputRotation))
            return;

        // Rotate the camera
        transform.eulerAngles = new Vector3(-_inputRotation.x, transform.eulerAngles.y + _inputRotation.y, 0f);

        // Set body._cameraForward to rotate the body according to the camera rotation
        characterBody.SetCameraForward(transform.forward);
    }

    private void MoveCamera()
    {
        // Reposition camera to follow the target
        transform.position = target.position - (transform.forward * _targetDistance);
    }

    public float GetCameraHorizontalSensitivity()
    {
        return cameraSensitivity;
    }

    public void SetInputRotation(Vector2 input)
    {
        _rawInputRotation = input;

        if (Vector2.zero.Equals(_rawInputRotation))
            return;

        // We invert them since horizontal rotation (input.x) happens on the Y axis, while vertical rotation (input.y) happens on the X axis
        float y = input.x * cameraSensitivity;
        _rotX += input.y * cameraSensitivity;

        // Clamp vertical rotation so the camera doesn't go through the floor or above max character rotation possibilities 
        _rotX = Mathf.Clamp(_rotX, verticalMinClamp, verticalMaxClamp);

        _inputRotation = new Vector2(_rotX, y);
    }
}