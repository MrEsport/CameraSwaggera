using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UnityEditor.CanEditMultipleObjects]
public class DollyView : AView
{
    [SerializeField, Range(-180f, 180f)] private float roll = 0;
    [SerializeField, Min(0f)] private float distance;
    [SerializeField, Range(0, 180f)] private float fov = 60;
    
    [SerializeField] private Transform target;
    [SerializeField] private Rail rail;

    [SerializeField] private bool isAuto;
    [SerializeField] private float speed;
    [HideIf(nameof(isAuto)), InputAxis, SerializeField] private string dollyDistanceInput;
    [HideIf(nameof(isAuto)), SerializeField] private bool invertInputAxis;

    private float _distanceOnRail;

    private void Update()
    {
        float direction = 0f;

        if (isAuto)
        {
            float targetDistance = rail.GetClosestPointOnRailDistance(target.position);

            if (Mathf.Abs(targetDistance - _distanceOnRail) <= 0.5f) direction = 0f;
            else if (!rail.IsLoop) direction = Mathf.Sign(targetDistance - _distanceOnRail);
            else
            {
                float diff = Mathf.Abs(targetDistance - _distanceOnRail);
                float loopedDiff = 0f;

                if (_distanceOnRail < targetDistance)
                {
                    loopedDiff = _distanceOnRail + rail.Length - targetDistance;
                    direction = Mathf.Sign(loopedDiff - diff);
                }
                else
                {
                    loopedDiff = targetDistance + rail.Length - _distanceOnRail;
                    direction = Mathf.Sign(diff - loopedDiff);
                }
            }
        }
        else
        {
            direction = Input.GetAxisRaw(dollyDistanceInput);
            if (invertInputAxis) direction *= -1f;
        }

        _distanceOnRail += direction * speed * Time.deltaTime;
        _distanceOnRail = Mathf.Repeat(_distanceOnRail, rail.Length);
    }

    public override CameraConfiguration GetConfiguration()
    {
        var config = base.GetConfiguration();

        config.pivot = rail.GetPosition(_distanceOnRail);

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
        if (target == null || rail == null || !rail.HasNodes)
            return;

        GetConfiguration().DrawGizmos(Color.magenta);
    }

    protected override void Reset()
    {
        base.Reset();
        fov = 60f;
    }
}
