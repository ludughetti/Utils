using UnityEngine;

public class CharacterAnimatorView : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] new private Rigidbody rigidbody;
    [SerializeField] private string horizontalMovSpeedParam = "move_speed";
    [SerializeField] private string directionXParam = "dir_x";
    [SerializeField] private string directionZParam = "dir_z";

    private Vector2 _direction = Vector2.zero;

    private void Update()
    {
        SetHorizontalMovementAnimations();
    }

    private void SetHorizontalMovementAnimations()
    {
        var velocity = rigidbody.velocity;
        velocity.y = 0;

        var speed = velocity.magnitude;

        animator.SetFloat(horizontalMovSpeedParam, speed);
        animator.SetFloat(directionXParam, _direction.x);
        animator.SetFloat(directionZParam, _direction.y);
    }

    public void SetMovementDirection(Vector2 input)
    {
        float xValue = Mathf.Clamp(input.x, -1f, 1f);
        float zValue = Mathf.Clamp(input.y, -1f, 1f);
        _direction = new(xValue, zValue);
        Debug.Log($"{name}: _direction is {_direction}");
    }
}