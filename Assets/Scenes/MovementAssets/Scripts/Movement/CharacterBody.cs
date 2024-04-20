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
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 groundedOffset = new(0f, 0.001f, 0f);
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private float jumpHeight = 10f;
    [SerializeField] private float jumpSpeedMultiplier = 0.75f;

    [Header("Movement")]
    [SerializeField] private float brakeMultiplier = 0.75f;
    [SerializeField] private float dragAmount = 3.5f;
    [SerializeField] private float dragAmountMultiplier = 0.1f;

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
        _isGrounded = Physics.CheckSphere(transform.position + groundedOffset, groundCheckDistance, groundLayer);
        _rotationSpeed = new(0f, cameraControl.GetCameraSensitivity(), 0f);
    }

    private void FixedUpdate()
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

        RotateBody();
        MoveBody();

        if (!_isGrounded)
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

    private void MoveBody()
    {
        if (_isBrakeRequested && _isGrounded)
        {
            _rigidbody.AddForce(-_rigidbody.velocity * brakeMultiplier, ForceMode.Impulse);
            _isBrakeRequested = false;
            Debug.Log($"{name}: Brake!");
        }

        if (!_currentMovement.IsValid() || _rigidbody.velocity.magnitude >= _currentMovement.GoalSpeed)
            return;

        /* Multiply input.x by transform.right to move on x axis and input.y by transform.forward to move on z axis */
        Vector3 directionVector = (_currentMovement.Direction.x * transform.right + _currentMovement.Direction.z * transform.forward) 
            * _currentMovement.Acceleration * GetMovementDragMultiplier();
        directionVector.y = 0f;
        _rigidbody.AddForce(directionVector, ForceMode.Force);
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