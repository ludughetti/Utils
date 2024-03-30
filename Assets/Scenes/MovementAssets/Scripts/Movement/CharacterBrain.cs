using System;
using UnityEngine;

/* This class controls a body */
public class CharacterBrain : MonoBehaviour
{
    [SerializeField] private CharacterBody body;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float speed = 10;
    [SerializeField] private float acceleration = 4;

    Vector3 _desiredDirection;

    private void OnEnable()
    {
        if(body == null)
        {
            Debug.LogError($"{name}: {nameof(body)} is null!" +
                "\nDisabling object to avoid errors.");
            enabled = false;
            return;
        }
        inputReader.onMovementInput += HandleMovementInput;
    }

    private void OnDisable()
    {
        inputReader.onMovementInput -= HandleMovementInput;
    }

    private void HandleMovementInput(Vector2 input)
    {
        if(_desiredDirection.magnitude > Mathf.Epsilon && input.magnitude < Mathf.Epsilon)
        {
            Debug.Log($"{nameof(_desiredDirection)} magnitude: {_desiredDirection.magnitude}\t{nameof(input)} magnitude: {input.magnitude}");
            body.RequestBrake();
        }

        _desiredDirection = new Vector3(input.x, 0f, input.y);
        body.SetMovement(new MovementRequest(_desiredDirection, speed, acceleration));
    }
}
