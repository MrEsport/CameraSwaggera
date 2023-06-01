using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour
{
    [SerializeField] private List<Transform> nodes = new List<Transform>();
    [SerializeField] private bool isLoop;
    public bool IsLoop { get => isLoop; }

    private float length;
    public float Length { get => length; private set => length = value; }

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
        if (!isLoop)
        {
            if (distanceFromStart < 0)
                return nodes[0].position;
            if (distanceFromStart > length)
                return nodes[nodes.Count - 1].position;
        }
        else
            distanceFromStart = Mathf.Repeat(distanceFromStart, length);

        float currentDistance = 0;
        for (int i = 0; i < nodes.Count; ++i)
        {
            float segmentDistance = Vector3.Distance(nodes[i].position, nodes[(i + 1) % nodes.Count].position);
            if (distanceFromStart < currentDistance + segmentDistance)
            {
                currentDistance = distanceFromStart - currentDistance;
                return Vector3.Lerp( nodes[i].position, nodes[(i + 1) % nodes.Count].position, currentDistance / segmentDistance); 
            }
            currentDistance += segmentDistance;
        }

        return nodes[0].position;
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
