using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/* This class interfaces with rigidBody to control a character's movement through forces */
[RequireComponent (typeof(Rigidbody))]
public class CharacterBody : MonoBehaviour
{
    [Header("Brain")]
    [SerializeField] private CharacterBrain brain;

    [Header("Camera")]
    [SerializeField] private CameraControl cameraControl;

    [Header("In Air")]
    [SerializeField] private Vector3 groundedOffset = new(0f, 0.001f, 0f);
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private float jumpHeight = 10f;
    [SerializeField] private float jumpSpeedMultiplier = 0.75f;

    [Header("Movement")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float brakeMultiplier = 0.75f;
    [SerializeField] private float dragAmount = 3.5f;
    [SerializeField] private float dragAmountMultiplier = 0.1f;
    [SerializeField] private float maxFloorDistance = 1f;
    [SerializeField] private float maxSlopeAngle = 45;

    private Rigidbody _rigidbody;
    private MovementRequest _currentMovement = MovementRequest.InvalidRequest;
    private bool _isBrakeRequested = false;
    private bool _isGrounded;
    private bool _isOnSlope;
    private bool _isOnUnclimbableSlope;
    private float _horizontalRotation = 0f;
    private RaycastHit _slopeHit;
    private Vector3 _rotationSpeed = Vector3.zero;

    private void Start()
    {
        if (!AreAllComponentsAssigned())
            enabled = false;

        _rigidbody = GetComponent<Rigidbody>();
        _isGrounded = Physics.CheckSphere(transform.position + groundedOffset, groundCheckDistance, groundLayer);
        _rotationSpeed = new(0f, cameraControl.GetCameraSensitivity(), 0f);
    }

    private void FixedUpdate()
    {
        UpdatePhysics();

        if (_isBrakeRequested && _isGrounded)
            Break();

        RotateBody();
        MoveBody();

        if (!_isGrounded || _isOnUnclimbableSlope)
        {
            _rigidbody.AddForce((gravityScale - 1) * _rigidbody.mass * Physics.gravity);
        }
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

    public float GetHorizontalVelocityNormalized()
    {
        Vector3 rbHorizontalVelocity = _rigidbody.velocity;
        rbHorizontalVelocity.y = 0f;
        return rbHorizontalVelocity.normalized.magnitude;
    }

    public bool GetIsGrounded()
    {
        return _isGrounded;
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

    private void UpdatePhysics()
    {
        // Check if target is on the ground
        _isGrounded = Physics.CheckSphere(transform.position + groundedOffset, groundCheckDistance, groundLayer);

        // Adjust drag depending on if character is grounded or not
        if (_isGrounded)
        {
            _rigidbody.drag = dragAmount;
        }
        else
        {
            _rigidbody.drag = 0f;
        }

        if (_isOnSlope && _isGrounded)
            _rigidbody.useGravity = false;
        else
            _rigidbody.useGravity = true;
    }

    private void Break()
    {
        _rigidbody.AddForce(-_rigidbody.velocity * brakeMultiplier, ForceMode.Impulse);
        _isBrakeRequested = false;
        Debug.Log($"{name}: Brake!");
    }

    private void MoveBody()
    {
        if (!_currentMovement.IsValid() || _rigidbody.velocity.magnitude >= _currentMovement.GoalSpeed)
            return;

        /* Multiply input.x by transform.right to move on x axis and input.y by transform.forward to move on z axis */
        Vector3 directionVector = (_currentMovement.Direction.x * transform.right + _currentMovement.Direction.z * transform.forward);

        if (IsOnSlope(directionVector - transform.up))
        {
            directionVector = Vector3.ProjectOnPlane(directionVector, _slopeHit.normal);
        }

        directionVector.y = 0f;

        if(!_isOnSlope || !_isOnUnclimbableSlope)
            _rigidbody.AddForce(directionVector * _currentMovement.Acceleration * GetMovementDragMultiplier(), ForceMode.Force);
    }

    private bool IsOnSlope(Vector3 directionVector)
    {
        if (Physics.Raycast(transform.position, directionVector - transform.up, out _slopeHit, maxFloorDistance, groundLayer))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            _isOnSlope = angle != 0f;
            _isOnUnclimbableSlope = angle > maxSlopeAngle;
        }

        return _isOnSlope && !_isOnUnclimbableSlope;
    }

    private void RotateBody()
    {
        // Rotate the rigidbody depending on mouse horizontal input
        Quaternion deltaRotation = Quaternion.Euler(_horizontalRotation * Time.fixedDeltaTime * _rotationSpeed);
        _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
    }

    public void Jump()
    {
        if (_isGrounded)
        {
            _isGrounded = false;

            Vector3 horizontalVelocity = new(0f, _rigidbody.velocity.magnitude, 0f);
            _rigidbody.AddForce(jumpHeight * jumpSpeedMultiplier * Vector3.up + horizontalVelocity, 
                ForceMode.Impulse);
            Debug.Log($"{name}: Jump executed");
        }
    }
     
    // TODO: Maybe it'd be better to have a method running in FixedUpdate to check frame Physics?
    private float GetMovementDragMultiplier()
    {
        return _isGrounded ? 1f : dragAmountMultiplier;
    }
}