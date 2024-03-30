using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorView : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private string speedParameter = "hor_speed";

    private void Update()
    {
        var velocity = rigidbody.velocity;
        velocity.y = 0;

        var speed = velocity.magnitude;
        animator.SetFloat(speedParameter, speed);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(velocity), speed);
    }
}
