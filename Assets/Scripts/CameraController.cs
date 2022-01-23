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

        Vector2 screenSizeInUnits = new Vector2(2 * Camera.main.orthographicSize * Screen.width / Screen.height, 2 * Camera.main.orthographicSize);

        if (player != null)
        {
            Vector3 target = player.transform.position + offset;
            if (Ground.TryGetInstance(out Ground ground))
            {
                Vector2 cameraLimits = (ground.Size - screenSizeInUnits)/2;
                target.x = Mathf.Clamp(target.x, -cameraLimits.x, cameraLimits.x);
                target.z = Mathf.Clamp(target.z, -cameraLimits.y, cameraLimits.y);
            }
            transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
        }
    }
}
