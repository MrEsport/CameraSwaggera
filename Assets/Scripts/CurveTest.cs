using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveTest : MonoBehaviour
{
    [SerializeField] private Curve curve;

    private void Update()
    {
    }

    private void OnDrawGizmos()
    {
        curve.DrawGizmo(Color.black, transform.localToWorldMatrix);

    }
}
