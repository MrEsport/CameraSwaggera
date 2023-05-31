using System;
using UnityEngine;

[Serializable]
public class CameraConfiguration
{
    [Range(-90f, 90f)] public float pitch;
    [Range(0, 360f)] public float yaw;
    [Range(-180f, 180f)] public float roll = 0;

    public Vector3 pivot;
    [Min(0f)] public float distance;

    [Range(0, 180f)] public float fov = 60;

    public Quaternion GetRotation { get => Quaternion.Euler(pitch, yaw, roll); }
    public Vector3 GetPosition {  get => GetRotation * (Vector3.back * distance) + pivot; }

    public void DrawGizmos(Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(pivot, 0.25f);
        Vector3 position = GetPosition;
        Gizmos.DrawLine(pivot, position);
        Gizmos.matrix = Matrix4x4.TRS(position, GetRotation, Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, fov, 0.5f, 0f, Camera.main.aspect);
        Gizmos.matrix = Matrix4x4.identity;
    }
}
