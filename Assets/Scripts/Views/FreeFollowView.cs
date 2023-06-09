using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeFollowView : AView
{
    [SerializeField] private Transform target;

    [SerializeField] private float[] pitchs;
    [SerializeField] private float[] rolls;
    [SerializeField] private float[] fovs;

    [InputAxis, SerializeField] private string yawControlAxis;
    [SerializeField] private float yawSpeed;

    [SerializeField] private Curve curve;
    [InputAxis, SerializeField] private string curveControlAxis;
    [SerializeField] private float curveSpeed;
    [SerializeField] private bool invertCurveAxis;

    [Min(0), SerializeField] private float distance;

    private float yaw;
    private float curvePosition;
    private Matrix4x4 matrix;

    private void Update()
    {
        if (target == null)
            return;

        float yawInput = Input.GetAxisRaw(yawControlAxis);
        float curveInput = Input.GetAxisRaw(curveControlAxis);

        if(yawInput != 0)
        {
            yaw += yawSpeed * Time.deltaTime * yawInput;
        }

        if(curveInput != 0)
        {
            if (invertCurveAxis) curveInput *= -1f;
            curvePosition += curveSpeed * Time.deltaTime * curveInput;
            curvePosition = Mathf.Clamp01(curvePosition);
        }
    }

    public override CameraConfiguration GetConfiguration()
    {
        CameraConfiguration config = new CameraConfiguration();

        transform.position = target.position;

        matrix = Matrix4x4.TRS(target.position, Quaternion.Euler(0, yaw, 0), target.localScale);
        config.pivot = curve.GetPosition(curvePosition, matrix);
        config.yaw = yaw;

        if(curvePosition > .5f)
        {
            float t = Mathf.InverseLerp(0.5f, 1, curvePosition);
            config.pitch = Mathf.Lerp(pitchs[1], pitchs[2], t);
            config.roll = Mathf.Lerp(rolls[1], rolls[2], t);
            config.fov = Mathf.Lerp(fovs[1], fovs[2], t);
        }
        else 
        {
            float t = Mathf.InverseLerp(0f, .5f, curvePosition);
            config.pitch = Mathf.Lerp(pitchs[0], pitchs[1], t);
            config.roll = Mathf.Lerp(rolls[0], rolls[1], t);
            config.fov = Mathf.Lerp(fovs[0], fovs[1], t);
        }

        config.distance = distance;

        return config;
    }

    private void OnDrawGizmos()
    {
        curve.DrawGizmo(Color.green, matrix);
    }
}
