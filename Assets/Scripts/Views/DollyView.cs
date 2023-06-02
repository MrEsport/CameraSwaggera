using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyView : AView
{
    [SerializeField] Transform target;
    [SerializeField] Rail rail;

    [SerializeField] float distanceOnRail;
    [SerializeField] float speed;

    [Range(-180f, 180f)] public float roll = 0;

    [Min(0f)] public float distance;

    [Range(0, 180f)] public float fov = 60;

    private void Update()
    {
        distanceOnRail += Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
        if (!rail.IsLoop) distanceOnRail = Mathf.Clamp(distanceOnRail, 0, rail.Length);
    }

    public override CameraConfiguration GetConfiguration()
    {
        var config = base.GetConfiguration();

        config.pivot = rail.GetPosition(distanceOnRail);

        Vector3 dir = (target.position - config.pivot).normalized;

        config.pitch = -Mathf.Asin(dir.y) * Mathf.Rad2Deg;
        config.yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        config.roll = roll;

        config.distance = distance;
        config.fov = fov;

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
