using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Player player;

    private Vector3 offset;

    private Vector3 velocity;


    [SerializeField] private float smoothTime = 1;

    private void Start()
    {
        offset = transform.localPosition;
    }

    protected void LateUpdate()
    {
        if (player != null)
        {
            Vector3 target = player.transform.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
        }
    }
}
