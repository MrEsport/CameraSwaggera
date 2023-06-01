using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    public static Vector3 LinearBezier(Vector3 A, Vector3 B, float t)
    {
        return Vector3.Lerp(A, B, t);
    }

    public static Vector3 QuadraticBezier(Vector3 A, Vector3 B, Vector3 C, float t)
    {

        return LinearBezier(LinearBezier(A,B,t), LinearBezier(B,C,t), t);
    }

    public static Vector3 CubicBezier(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t)
    {

        return LinearBezier(QuadraticBezier(A,B,C,t), QuadraticBezier(B,C,D,t),t );
    }

}
