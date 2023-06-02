using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeFollowView : AView
{
    [SerializeField] private float[] pitchs;
    [SerializeField] private float[] rolls;
    [SerializeField] private float[] fovs;

    [SerializeField] private float yaw;
    [SerializeField] private float yawSpeed;

    [SerializeField] private Transform target;

    [SerializeField] private Curve curve;
    [SerializeField] private float curvePosition;
    [SerializeField] private float curveSpeed;

    private Matrix4x4 matrix;

    private void Update()
    {
        if (target == null)
            return;

        if(Input.GetAxis("Horizontal") != 0)
        {
            yaw += yawSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        }

        if(Input.GetAxis("Vertical") != 0)
        {
            curvePosition += curveSpeed * Time.deltaTime * Input.GetAxis("Vertical");
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


        return config;
    }

    private void OnDrawGizmos()
    {
        curve.DrawGizmo(Color.green, matrix);
    }
}
