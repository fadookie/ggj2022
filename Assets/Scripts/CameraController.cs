using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Player player;

    private Vector3 offset;

    private Vector3 velocity;

    private Vector2 screenSizeInUnits;

    [SerializeField] private float smoothTime = 1;


    private static CameraController instance;

    protected void Start()
    {
        instance = this;
        offset = transform.localPosition;
    }

    protected void LateUpdate()
    {
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
    public Bounds GetScreenInWorldSpace()
    {
        Vector2 screenSizeInUnits = new Vector2(2 * Camera.main.orthographicSize * Screen.width / Screen.height, 2 * Camera.main.orthographicSize);
        return new Bounds(transform.position, new Vector3(screenSizeInUnits.x, 9999, screenSizeInUnits.y));
    }

    public static bool TryGetInstance(out CameraController cameraController)
    {
        cameraController = instance;
        return cameraController != null;
    }
}
