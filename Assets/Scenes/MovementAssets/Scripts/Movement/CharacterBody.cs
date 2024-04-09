using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/* This class interfaces with rigidBody to control a character's movement through forces */
[RequireComponent (typeof(Rigidbody))]
public class CharacterBody : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private CameraControl cameraControl;
    [SerializeField] private float brakeMultiplier = 0.75f;
    [SerializeField] private float characterHeight = 1.8f;
    [SerializeField] private float dragAmount = 3.5f;
    [SerializeField] private Vector3 groundedOffset = new(0, 0.001f, 0);

    private Rigidbody _rigidbody;
    private MovementRequest _currentMovement = MovementRequest.InvalidRequest;
    private bool _isBrakeRequested = false;
    private bool _isGrounded;
    private float _horizontalRotation = 0f;
    private Vector3 _rotationSpeed = Vector3.zero;

    private void Start()
    {
        if (!AreAllComponentsAssigned())
            enabled = false;

        _rigidbody = GetComponent<Rigidbody>();
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, characterHeight * 0.5f + 0.2f, groundLayer);
        _rotationSpeed = new(0f, cameraControl.GetCameraSensitivity(), 0f);
    }

    private void Update()
    {
        // Check if target is on the ground
        _isGrounded = Physics.Raycast(transform.position + groundedOffset, 
            Vector3.down, characterHeight * 0.5f + 0.2f, groundLayer);

        // Adjust drag depending on if character is grounded or not
        if (_isGrounded)
            _rigidbody.drag = dragAmount;
        else
            _rigidbody.drag = 0f;
    }

    private void FixedUpdate()
    {
        RotateBody();
        MoveCharacter();
    }

    private void Reset()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void SetMovement(MovementRequest movementRequest)
    {
        _currentMovement = movementRequest;
    }

    public void SetHorizontalRotation(float inputRotation)
    {
        _horizontalRotation = inputRotation;
    }

    public void RequestBrake()
    {
        _isBrakeRequested = true;
    }

    // Check if all dependencies are properly set in the UI
    private bool AreAllComponentsAssigned()
    {
        if (groundLayer == 0)
        {
            Debug.Log($"{name}: Ground layer is not assigned");
            return false;
        }

        if (!cameraControl)
        {
            Debug.Log($"{name}: Camera control is not assigned");
            return false;
        }

        return true;
    }

    private void MoveCharacter()
    {
        if (_isBrakeRequested)
        {
            _rigidbody.AddForce(-_rigidbody.velocity * brakeMultiplier, ForceMode.Impulse);
            _isBrakeRequested = false;
            Debug.Log($"{name}: Brake!");
        }

        if (!_currentMovement.IsValid() || _rigidbody.velocity.magnitude >= _currentMovement.GoalSpeed)
            return;

        /* Multiply input.x by transform.right to move on x axis and input.y by transform.forward to move on z axis */
        Vector3 directionVector = (_currentMovement.Direction.x * transform.right + _currentMovement.Direction.z * transform.forward) * _currentMovement.Acceleration;
        directionVector.y = 0f;
        _rigidbody.AddForce(directionVector, ForceMode.Force);
    }

    private void RotateBody()
    {
        // Rotate the rigidbody depending on mouse horizontal input
        Quaternion deltaRotation = Quaternion.Euler(_horizontalRotation * Time.fixedDeltaTime * _rotationSpeed);
        _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
    }
}