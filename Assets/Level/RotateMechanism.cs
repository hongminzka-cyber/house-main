using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMechanism : MonoBehaviour
{
    public enum RotateAxis
    {
        X, Y, Z
    }

    public RotateAxis rotateAxis = RotateAxis.Z;
    public float rotateAngle = 90f;
    public float rotateTime = 0.2f;
    public int direction = 1;
    public LayerMask blockingMask = ~0;
    public float overlapShrink = 0.01f;

    public bool blockWhenPlayerOnTop = true;

    bool isRotating = false;
    Quaternion lastRotation;
    BoxCollider[] childColliders;

    public bool IsRotating => isRotating;

    void Awake()
    {
        childColliders = GetComponentsInChildren<BoxCollider>();
    }

    public void TryRotate()
    {
        if (isRotating) return;

        if (blockWhenPlayerOnTop && HasPlayerOnMechanism())
        {
            Debug.Log("Mechanism blocked because player is on it.");
            return;
        }

        StartCoroutine(RotateRoutine());
    }

    bool HasPlayerOnMechanism()
    {
        return GetComponentInChildren<Player>() != null;
    }

    IEnumerator RotateRoutine()
    {
        isRotating = true;

        lastRotation = transform.rotation;
        Quaternion targetRotation =
            lastRotation * Quaternion.AngleAxis(rotateAngle * direction, GetAxisVector());

        yield return RotateTo(lastRotation, targetRotation, rotateTime);

        Physics.SyncTransforms();

        if (IsBlocked())
        {
            yield return RotateTo(targetRotation, lastRotation, rotateTime * 0.8f);
            Physics.SyncTransforms();
        }

        isRotating = false;
    }

    IEnumerator RotateTo(Quaternion from, Quaternion to, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            t = t * t * (3f - 2f * t);

            transform.rotation = Quaternion.Slerp(from, to, t);
            yield return null;
        }

        transform.rotation = to;
    }

    bool IsBlocked()
    {
        foreach (var box in childColliders)
        {
            if (box == null || !box.enabled) continue;

            Vector3 center = box.transform.TransformPoint(box.center);
            Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, Abs(box.transform.lossyScale));

            halfExtents = new Vector3(
                Mathf.Max(halfExtents.x - overlapShrink, 0.001f),
                Mathf.Max(halfExtents.y - overlapShrink, 0.001f),
                Mathf.Max(halfExtents.z - overlapShrink, 0.001f)
            );

            Collider[] hits = Physics.OverlapBox(
                center,
                halfExtents,
                box.transform.rotation,
                blockingMask,
                QueryTriggerInteraction.Ignore
            );

            foreach (var hit in hits)
            {
                if (hit == null) continue;
                if (hit.transform.IsChildOf(transform)) continue;
                return true;
            }
        }

        return false;
    }

    Vector3 GetAxisVector()
    {
        switch (rotateAxis)
        {
            case RotateAxis.X: return Vector3.right;
            case RotateAxis.Y: return Vector3.up;
            default: return Vector3.forward;
        }
    }

    Vector3 Abs(Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }
}