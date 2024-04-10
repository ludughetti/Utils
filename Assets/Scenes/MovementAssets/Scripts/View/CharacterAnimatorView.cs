using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class CharacterAnimatorView : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] new private Rigidbody rigidbody;

    [Header("Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private float animationSpeed = 4f;
    [SerializeField] private string directionXParam = "dir_x";
    [SerializeField] private string directionZParam = "dir_z";
    [SerializeField] private string horizontalMovSpeedParam = "move_speed";

    private Vector2 _currentInput = Vector2.zero;
    private Vector2 _nextDirection = Vector2.zero;
    private Vector2 _previousDirection = Vector2.zero;

    private void Update()
    {
        SetMovementAnimations();
    }

    private void SetMovementAnimations()
    {
        SmoothMovementValues();

        animator.SetFloat(directionXParam, _currentInput.x);
        animator.SetFloat(directionZParam, _currentInput.y);
    }

    /*
     * This method lerps input(X,Y) values between changes to smooth animation transitions.
     * If a change comes from a negative position (e.g. [-1,0]) to a positive one (e.g. [1,0]), 
     * then the current position needs to be increased.
     * However, if a change comes from a positive position (e.g. [1,0]) to a negative one (e.g. [-1,0]), 
     * then the current position needs to be decreased.
     * 
     * _previousDirection will be updated once _currentInput reaches _nextDirection's values 
     * (AKA the actual input value provided by InputReader).
     * 
     * This is calculated both for input.x as well as input.y.
     */
    private void SmoothMovementValues()
    {
        // Lerp on X
        if (_nextDirection.x > _previousDirection.x)
        {
            _currentInput.x += Time.deltaTime * animationSpeed;
            if(_currentInput.x >= _nextDirection.x)
                _previousDirection.x = _nextDirection.x;
        }
        else if(_nextDirection.x < _previousDirection.x)
        {
            _currentInput.x -= Time.deltaTime * animationSpeed;
            if (_currentInput.x <= _nextDirection.x)
                _previousDirection.x = _nextDirection.x;
        }

        // Lerp on Y
        if (_nextDirection.y > _previousDirection.y)
        {
            _currentInput.y += Time.deltaTime * animationSpeed;
            if(_currentInput.y >= _nextDirection.y)
                _previousDirection.y = _nextDirection.y;
        }
        else if (_nextDirection.y < _previousDirection.y)
        {
            _currentInput.y -= Time.deltaTime * animationSpeed;
            if (_currentInput.y <= _nextDirection.y)
                _previousDirection.y = _nextDirection.y;
        }  
    }

    public void SetMovementDirection(Vector2 input)
    {
        _nextDirection = input;
    }
}