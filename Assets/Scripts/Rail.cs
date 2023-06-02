using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rail : MonoBehaviour
{
    [SerializeField] private List<Transform> nodes = new List<Transform>();
    [SerializeField] private bool isLoop;
    public bool IsLoop { get => isLoop; }

    private float _length = 0f;
    public float Length { get => _length; private set => _length = value; }

    public bool HasNodes { get => nodes != null && nodes.Count > 0; }

    private void Start()
    {
        GetLength();
    }

    private void GetLength()
    {
        for (int i = 0; i < nodes.Count; ++i)
        {
            if (i == nodes.Count - 1 && !isLoop)
                break;
            Length += Vector3.Distance(nodes[i].position, nodes[(i + 1) % nodes.Count].position);
        }
    }

    public Vector3 GetPosition(float distanceFromStart)
    {
        if (distanceFromStart < 0)
            return nodes[0].position;
        if (distanceFromStart > _length)
            return nodes[nodes.Count - 1].position;

        float currentDistance = 0;
        for (int i = 0; i < nodes.Count; ++i)
        {
            float segmentDistance = Vector3.Distance(nodes[i].position, nodes[(i + 1) % nodes.Count].position);
            if (distanceFromStart < currentDistance + segmentDistance)
            {
                currentDistance = distanceFromStart - currentDistance;
                return Vector3.Lerp(nodes[i].position, nodes[(i + 1) % nodes.Count].position, currentDistance / segmentDistance);
            }
            currentDistance += segmentDistance;
        }

        throw new NullReferenceException($"'{nameof(nodes)}' List is Empty");
    }

    public Vector3 GetClosestPointOnRail(Vector3 target)
    {
        if (nodes.Count <= 0)
            return target;
        else if (nodes.Count == 1)
            return nodes[0].position;

        Vector3 closestPoint = nodes[0].position;
        for (int i = 0; i < nodes.Count; ++i)
        {
            if (i == nodes.Count - 1 && !isLoop)
                break;

            var point = MathUtils.GetNearestPointOnSegment(nodes[i].position, nodes[(i + 1) % nodes.Count].position, target);
            if(Vector3.Distance(target, point) < Vector3.Distance(closestPoint, target))
                closestPoint = point;
        }

        return closestPoint;
    }

    public float GetClosestPointOnRailDistance(Vector3 target)
    {
        if (nodes.Count <= 0 || nodes.Count == 1)
            return 0f;

        float distance = 0f, closestDistance = 0f;
        Vector3 closestPoint = nodes[0].position;
        for (int i = 0; i < nodes.Count; ++i)
        {
            if (i == nodes.Count - 1 && !isLoop)
                break;

            var point = MathUtils.GetNearestPointOnSegment(nodes[i].position, nodes[(i + 1) % nodes.Count].position, target);
            if (Vector3.Distance(target, point) < Vector3.Distance(closestPoint, target))
            {
                closestPoint = point;
                closestDistance = distance + Vector3.Distance(nodes[i].position, closestPoint);
            }
            distance += Vector3.Distance(nodes[i].position, nodes[(i + 1) % nodes.Count].position);
        }

        return closestDistance;
    }

    private void OnDrawGizmos()
    {
        if (nodes.Count <= 0)
            return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < nodes.Count; ++i)
        {
            Gizmos.DrawWireSphere(nodes[i].position, 0.1f);
            if (i == nodes.Count - 1 && !isLoop)
                break;
            Gizmos.DrawLine(nodes[i].position, nodes[(i + 1) % nodes.Count].position);
        }
    }
}
