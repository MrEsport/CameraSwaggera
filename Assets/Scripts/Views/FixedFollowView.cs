using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FixedFollowView : AView
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform centralPoint;

    [SerializeField, Range(-180f, 180f)] private float roll;
    [SerializeField, Range(0, 180f)] private float fov;

    [SerializeField, Range(0, 90f)] private float pitchOffsetMax;
    [SerializeField, Range(0, 180f)] private float yawOffsetMax;

    [Min(0f)] public float distance;

    protected override void Start()
    {
        base.Start();
        if (centralPoint == null)
            centralPoint = target;
    }

    public override CameraConfiguration GetConfiguration()
    {
        var config = base.GetConfiguration();

        Vector3 dir = (target.position - transform.position).normalized;
        Vector3 centralDir = centralPoint == null ? dir : (centralPoint.position - transform.position).normalized;

        float diff = (-Mathf.Asin(dir.y) * Mathf.Rad2Deg) - (-Mathf.Asin(centralDir.y) * Mathf.Rad2Deg);
        config.pitch = (Mathf.Abs(diff) < pitchOffsetMax) ?
            -Mathf.Asin(dir.y) * Mathf.Rad2Deg :
            -Mathf.Asin(centralDir.y) * Mathf.Rad2Deg + pitchOffsetMax * Mathf.Sign(diff);

        diff = (Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg) - (Mathf.Atan2(centralDir.x, centralDir.z) * Mathf.Rad2Deg);

        // mod(360) for yaw cases under -180° OR over 180°
        diff %= 360;
        if (diff < -180f) diff += 360f;
        else if (diff > 180f) diff -= 360f;

        config.yaw = (Mathf.Abs(diff) < yawOffsetMax) ?
            Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg :
            Mathf.Atan2(centralDir.x, centralDir.z) * Mathf.Rad2Deg + yawOffsetMax * Mathf.Sign(diff);
        
        config.roll = roll;

        config.fov = fov;

        config.pivot = transform.position;
        config.distance = distance;

        return config;
    }

    private void OnDrawGizmos()
    {
        if (target == null)
            return;

        GetConfiguration().DrawGizmos(Color.magenta);
    }

    protected override void Reset()
    {
        base.Reset();
        fov = 60f;
    }
}
