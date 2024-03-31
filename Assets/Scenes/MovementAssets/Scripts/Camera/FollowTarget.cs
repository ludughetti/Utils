using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 cameraOffset = new(0, 3.5f, -5f);
    [SerializeField] private float speedMultiplier = 10;

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
        var directionToTarget = (target.position + cameraOffset) - transform.position;
        transform.position += directionToTarget * (speedMultiplier * Time.deltaTime);

    }
}
