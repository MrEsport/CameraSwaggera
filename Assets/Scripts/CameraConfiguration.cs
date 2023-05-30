using UnityEngine;

[CreateAssetMenu(menuName = "Camera/Camera Configuration")]
public class CameraConfiguration : ScriptableObject
{
    public float pitch;
    public float yaw;
    public float roll = 0;

    public Vector3 pivot;
    public float distance;

    public float fov = 90;

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
