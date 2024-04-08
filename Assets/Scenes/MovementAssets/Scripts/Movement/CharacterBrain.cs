using UnityEngine;

/* This class controls a body */
public class CharacterBrain : MonoBehaviour
{
    [SerializeField] private CharacterBody body;
    [SerializeField] new private CameraControl camera;
    [SerializeField] private CharacterAnimatorView view;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float speed = 10;
    [SerializeField] private float acceleration = 4;
    
    private float _reducedAcceleration;
    private float _reducedSpeed;
    private Vector3 _desiredDirection = Vector3.zero;

    private void Start()
    {
        _reducedAcceleration = acceleration - (acceleration / 3);
        _reducedSpeed = speed - (speed / 2);
    }

    private void OnEnable()
    {
        if(body == null)
        {
            Debug.LogError($"{name}: {nameof(body)} is null!" +
                "\nDisabling object to avoid errors.");
            enabled = false;
            return;
        }

        inputReader.OnMovementInput += HandleMovementInput;
        inputReader.OnCameraInput += HandleCameraInput;
    }

    private void OnDisable()
    {
        inputReader.OnMovementInput -= HandleMovementInput;
        inputReader.OnCameraInput -= HandleCameraInput;
    }

    private void HandleMovementInput(Vector2 input)
    {
        if(_desiredDirection.magnitude > Mathf.Epsilon && input.magnitude < Mathf.Epsilon)
        {
            Debug.Log($"{nameof(_desiredDirection)} magnitude: {_desiredDirection.magnitude}\t{nameof(input)} magnitude: {input.magnitude}");
            body.RequestBrake();
        }

        _desiredDirection = new Vector3(input.x, 0f, input.y);
        body.SetMovement(new MovementRequest(_desiredDirection, getSpeedByDirection(input), getAccelerationByDirection(input)));
        view.SetMovementDirection(input);
    }

    private void HandleCameraInput(Vector2 input)
    {
        body.SetHorizontalRotation(input.x);
        camera.SetInputRotation(input.y);
    }

    // If character is moving forward, use full speed. Else, reduced.
    private float getSpeedByDirection(Vector2 input)
    {
        if (input.y > 0f)
            return speed;

        return _reducedSpeed;
    }

    // If character is moving forward, use full acceleration. Else, reduced.
    private float getAccelerationByDirection(Vector2 input)
    {
        if (input.y > 0f)
            return acceleration;

        return _reducedAcceleration;
    }
}
