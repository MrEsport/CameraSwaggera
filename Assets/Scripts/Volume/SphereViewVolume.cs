using NaughtyAttributes;
using System;
using UnityEngine;

public class SphereViewVolume : AViewVolume
{
    [SerializeField] private Transform target;

    [MinValue(0), SerializeField] private float outerRadius;
    [MinValue(0), SerializeField] private float innerRadius;

    [SerializeField, CurveRange(EColor.Green)] private AnimationCurve blendCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float distance;

    private void Update()
    {
        if (target == null)
            throw new NullReferenceException($"Null Reference : {nameof(target)} reference not found");

        distance = Vector3.Distance(transform.position, target.position);

        if (distance <= outerRadius && !IsActive)
            SetActive(true);
        else if (distance > outerRadius && IsActive)
            SetActive(false);
    }

    public override float ComputeSelfWeight()
    {
        return blendCurve.Evaluate(Mathf.InverseLerp(outerRadius, innerRadius, distance));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(.25f, .55f, .1f);
        Gizmos.DrawWireSphere(transform.position, innerRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, outerRadius);
    }

    private void OnValidate()
    {
        outerRadius = Mathf.Max(outerRadius, innerRadius);
    }
}
