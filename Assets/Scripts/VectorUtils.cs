using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorUtils
{
    public static Vector2 ToVector2(this Vector3 vector3) {
        return new Vector2(vector3.x, vector3.z);
    }

    public static Vector3 ToVector3(this Vector2 vector2) {
        return new Vector3(vector2.x, 0, vector2.y);
    }
}
