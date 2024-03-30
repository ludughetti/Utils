using UnityEngine;

public class RotateCharacterBasedOnVelocity : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private float rotationSpeed = 1;

    private void Update()
    {
        var velocity = rigidbody.velocity;
        if (velocity.magnitude < Mathf.Epsilon)
            return;

        // The angle is always positive, we need to decide whether we need it to be negative or not
        var rotationAngle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
        transform.Rotate(Vector3.up, rotationAngle * rotationSpeed * Time.deltaTime);
    }
}
