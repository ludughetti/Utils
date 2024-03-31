using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 cameraOffset = new(0, 3.5f, -5f);
    [SerializeField] private float cameraSensitivity = 10;

    private Vector2 cameraRotation = Vector2.zero;

    private void Awake()
    {
        if (!target)
        {
            Debug.LogError($"{name}: Target is null!");
            enabled = false;
        }
    }

    private void Update()
    {
        /*var directionToTarget = (target.position + cameraOffset) - transform.position;
        transform.position += directionToTarget * (speedMultiplier * Time.deltaTime);*/

        Debug.Log($"{name}: Camera rotation vector2 is {cameraRotation})");

    }

    private void RotateCameraOnInput()
    {

    }

    private void MoveCameraOnRotation()
    {

    }

    public void SetRotation(Vector2 input)
    {
        cameraRotation = input * cameraSensitivity;
    }
}
