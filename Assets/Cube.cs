using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public List<Cube> neighbors = new List<Cube>();

    [Header("Optional custom standing point")]
    public Transform standPoint;

    public Vector3 GetTopCenter()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Bounds b = col.bounds;
            return new Vector3(b.center.x, b.max.y, b.center.z);
        }

        return transform.position + Vector3.up * 0.5f;
    }

    public Vector3 GetStandPosition(float standingHeight = 0.4f)
    {
        if (standPoint != null)
            return standPoint.position;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Bounds b = col.bounds;
            return new Vector3(b.center.x, b.max.y + standingHeight, b.center.z);
        }

        return transform.position + Vector3.up * standingHeight;
    }

    public Quaternion GetStandRotation()
    {
        if (standPoint != null)
            return standPoint.rotation;

        return Quaternion.identity;
    }

    void OnDrawGizmos()
    {
        Vector3 p = standPoint != null ? standPoint.position : GetStandPosition(0f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(p, 0.12f);

        if (neighbors == null) return;

        Gizmos.color = Color.yellow;
        foreach (var n in neighbors)
        {
            if (n != null)
            {
                Vector3 np = n.standPoint != null ? n.standPoint.position : n.GetStandPosition(0f);
                Gizmos.DrawLine(p, np);
            }
        }
    }
}