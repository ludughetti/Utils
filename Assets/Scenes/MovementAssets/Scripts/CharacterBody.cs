using System;
using UnityEngine;

/* This class interfaces with rigidBody to control a character's movement through forces */
[RequireComponent (typeof(Rigidbody))]
public class CharacterBody : MonoBehaviour
{
    [SerializeField]
    private float brakeMultiplier;

    private Rigidbody _rigidbody;
    private MovementRequest _currentMovement = MovementRequest.InvalidRequest;
    private bool _isBrakeRequested = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(_isBrakeRequested)
        {
            _rigidbody.AddForce(-_rigidbody.velocity * brakeMultiplier, ForceMode.Impulse);
            _isBrakeRequested = false;
            Debug.Log($"{name}: Brake!");
        }

        if (!_currentMovement.IsValid() || _rigidbody.velocity.magnitude >= _currentMovement.GoalSpeed)
            return;

        _rigidbody.AddForce(_currentMovement.GetAccelerationVector(), ForceMode.Force);
    }

    private void OnValidate()
    {
        _rigidbody = _rigidbody != null ? _rigidbody : GetComponent<Rigidbody>();
    }

    private void Reset()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void SetMovement(MovementRequest movementRequest)
    {
        _currentMovement = movementRequest;
    }

    public void RequestBrake()
    {
        _isBrakeRequested = true;
    }
}
