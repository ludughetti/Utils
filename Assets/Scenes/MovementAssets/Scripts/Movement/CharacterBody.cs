using UnityEngine;

/* This class interfaces with rigidBody to control a character's movement through forces */
[RequireComponent (typeof(Rigidbody))]
public class CharacterBody : MonoBehaviour
{
    [SerializeField] private float brakeMultiplier;
    [SerializeField] private float characterHeight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float dragAmount;
    [SerializeField] private float horizontalRotation = 1f;

    private Rigidbody _rigidbody;
    private MovementRequest _currentMovement = MovementRequest.InvalidRequest;
    private bool _isBrakeRequested = false;
    private bool _isGrounded;
    private Vector3 _cameraForward = Vector3.zero;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, characterHeight * 0.5f + 0.2f, groundLayer);
    }

    private void Update()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, characterHeight * 0.5f + 0.2f, groundLayer);
        //Debug.Log($"{name}: _isGrounded {_isGrounded}");

        if (_isGrounded)
        {
            _rigidbody.drag = dragAmount;
        }
        else
        {
            _rigidbody.drag = 0f;
        }
    }

    private void FixedUpdate()
    {
        RotateCharacter();
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
        horizontalRotation = inputRotation;
    }

    public void RequestBrake()
    {
        _isBrakeRequested = true;
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

    private void RotateCharacter()
    {
        //transform.Rotate(Vector3.up, _horizontalRotation * Time.fixedDeltaTime);

        // Debug.Log($"{name}: Torque rotation is {Vector3.up * _horizontalRotation * Time.fixedDeltaTime}");
        // _rigidbody.AddTorque(Vector3.up * _horizontalRotation * Time.fixedDeltaTime, ForceMode.Acceleration);

        if (Vector3.zero.Equals(_cameraForward))
            return;

        /*Quaternion rotationQuaternion = Quaternion.LookRotation(_cameraForward);
        Debug.Log($"{name}: rotationQuaternion is {rotationQuaternion}");
        _rigidbody.MoveRotation(rotationQuaternion);*/
        Vector3 horizontalDirection = Vector3.ProjectOnPlane(_cameraForward, Vector3.up);
        Quaternion rotation = Quaternion.LookRotation(horizontalDirection);
        Quaternion bodyRotation = Quaternion.Slerp(_rigidbody.rotation, rotation, horizontalRotation);
        _rigidbody.MoveRotation(bodyRotation);
    }

    public void SetCameraForward(Vector3 cameraForward)
    {
        Debug.Log($"{name}: cameraForward is {cameraForward}");
        // Body will only rotate horizontally (on Y axis)
        _cameraForward = cameraForward;
    }
}
