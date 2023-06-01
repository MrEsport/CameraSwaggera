using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Curve : MonoBehaviour
{
    [SerializeField] private Vector3 A;
    [SerializeField] private Vector3 B;
    [SerializeField] private Vector3 C;
    [SerializeField] private Vector3 D;
    [SerializeField, Range(0f, 1f)] private float t;
    [SerializeField] private int curvePoints = 5;
    public Vector3 GetPosition(float t)
    {
        return MathUtils.CubicBezier(A, B, C, D, t);
    }
    public Vector3 GetPosition(float t, Matrix4x4 localToWorldMatrix)
    {
        return localToWorldMatrix.MultiplyPoint(MathUtils.CubicBezier(A, B, C, D, t));
    }

    public void DrawGizmo(Color c, Matrix4x4 localToWorldMatrix)
    {

        Gizmos.color = c;
        Gizmos.DrawSphere( localToWorldMatrix.MultiplyPoint(A), .1f);
        Gizmos.DrawSphere(localToWorldMatrix.MultiplyPoint(B), .1f);
        Gizmos.DrawSphere(localToWorldMatrix.MultiplyPoint(C), .1f);
        Gizmos.DrawSphere(localToWorldMatrix.MultiplyPoint(D), .1f);

        Vector3 previousPoint = Vector3.zero;
        float dis = 1f / curvePoints;
        for (int i = 0; i < curvePoints; i++)
        {
            previousPoint = GetPosition(dis * i, localToWorldMatrix);
            Gizmos.DrawLine(previousPoint, GetPosition(dis * (i +1), localToWorldMatrix));
        }
        Gizmos.DrawLine(previousPoint, GetPosition(1, localToWorldMatrix));
        Gizmos.color = c;
        Gizmos.DrawSphere(GetPosition(t, localToWorldMatrix), .2f);
    }

}
