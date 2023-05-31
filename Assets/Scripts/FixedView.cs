using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UnityEditor.CanEditMultipleObjects]     
public class FixedView : AView
{
    [SerializeField, Range(-90f, 90f)] private float pitch;
    [SerializeField, Range(0, 360f)] private float yaw;
    [SerializeField, Range(-180f, 180f)] private float roll;

    [SerializeField, Range(0, 180f)] private float fov;

    public override CameraConfiguration GetConfiguration()
    {
        var config = base.GetConfiguration();

        config.pitch = pitch;   
        config.yaw = yaw;
        config.roll = roll;
        config.fov = fov;

        config.pivot = transform.position;
        config.distance = 0;

        return config;
    }

    private void OnDrawGizmos()
    {
        GetConfiguration().DrawGizmos(Color.magenta);
    }

    protected override void Reset()
    {
        base.Reset();
        fov = 60f;
    }
}
