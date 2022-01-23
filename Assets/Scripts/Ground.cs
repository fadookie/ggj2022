using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Ground : MonoBehaviour
{
    private static Ground instance;

    public Rect Bounds => bounds;
    public Vector2 Size 
    {
        get => bounds.size;
        set
        {
            bounds.size = value;
            bounds.center = Vector2.zero;
            var parentScale = transform.parent?.lossyScale ?? Vector3.one;
            transform.localScale = new Vector3(value.x / 10 / parentScale.x, transform.localScale.y, value.y / 10 / parentScale.z);
        }
    }

    private Rect bounds;

    private Vector3 lastScale;

    protected void OnEnable()
    {
        instance = this;
    }

    protected void OnDisable()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    protected void Update()
    {
        if(transform.lossyScale != lastScale)
        {
            lastScale = transform.lossyScale;
            bounds.size = new Vector2(lastScale.x * 10, lastScale.z * 10);
            bounds.center = Vector2.zero;
        }
    }

    public static bool TryGetInstance(out Ground ground)
    {
        ground = instance;
        return ground != null;
    }
}
