using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SliderMechanism : MonoBehaviour
{
    public enum SlideAxis
    {
        X,
        Z
    }

    [Header("Slide")]
    public SlideAxis slideAxis = SlideAxis.X;
    public float minLocalValue = -2f;
    public float maxLocalValue = 2f;

    [Header("Gameplay")]
    public bool blockWhenPlayerOnTop = false;
    public PlayerSystem playerSystem;

    [Header("Connections")]
    public Cube frontEndCube;
    public Cube backEndCube;
    public float connectDistance = 0.6f;
    public float connectHeightTolerance = 0.6f;

    [Header("Debug")]
    public bool showDebugLog = false;

    private bool isDragging = false;
    private Transform activeRayOrigin;

    private float startRayAxisValue;
    private float startSliderAxisValue;

    private Cube[] allCubes;

    public bool IsBusy => isDragging;

    void Start()
    {
        allCubes = Object.FindObjectsOfType<Cube>();
        RefreshEndpointConnections();
    }

    public void StartDragging(Transform rayOrigin)
    {
        if (isDragging) return;
        if (rayOrigin == null) return;

        if (playerSystem != null && playerSystem.IsMoving)
            return;

        if (blockWhenPlayerOnTop && HasPlayerOnMechanism())
        {
            if (showDebugLog)
                Debug.Log("Slider blocked because player is standing on it.");
            return;
        }

        if (!TryGetControllerAxisValue(rayOrigin, out startRayAxisValue))
        {
            if (showDebugLog)
                Debug.LogWarning("Slider could not start drag: ray did not hit drag plane.");
            return;
        }

        activeRayOrigin = rayOrigin;
        startSliderAxisValue = GetAxisValue(transform.localPosition);
        isDragging = true;

        if (showDebugLog)
        {
            Debug.Log(
                "[Slider StartDragging] axis = " + slideAxis +
                " | startRayAxis = " + startRayAxisValue +
                " | startSliderAxis = " + startSliderAxisValue +
                " | min = " + GetSafeMin() +
                " | max = " + GetSafeMax()
            );
        }
    }

    public void StopDragging()
    {
        if (!isDragging) return;

        isDragging = false;
        activeRayOrigin = null;
        RefreshEndpointConnections();

        if (showDebugLog)
            Debug.Log("[Slider StopDragging] localPosition = " + transform.localPosition);
    }

    void Update()
    {
        OVRInput.Update();

        if (!isDragging || activeRayOrigin == null)
            return;

        if (!TryGetControllerAxisValue(activeRayOrigin, out float currentRayAxis))
            return;

        float delta = currentRayAxis - startRayAxisValue;

        float targetValue = Mathf.Clamp(
            startSliderAxisValue + delta,
            GetSafeMin(),
            GetSafeMax()
        );

        Vector3 pos = transform.localPosition;
        pos = SetAxisValue(pos, targetValue);
        transform.localPosition = pos;

        RefreshEndpointConnections();

        if (showDebugLog)
        {
            Debug.Log(
                "[Slider Dragging] currentRayAxis = " + currentRayAxis +
                " | delta = " + delta +
                " | targetValue = " + targetValue +
                " | localPos = " + transform.localPosition
            );
        }
    }

    bool HasPlayerOnMechanism()
    {
        return GetComponentInChildren<Player>() != null;
    }

    bool TryGetControllerAxisValue(Transform rayOrigin, out float axisValue)
    {
        axisValue = 0f;

        Plane dragPlane = new Plane(Vector3.up, transform.position);
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        if (!dragPlane.Raycast(ray, out float enter))
            return false;

        Vector3 hitPoint = ray.GetPoint(enter);

        Vector3 local;
        if (transform.parent != null)
            local = transform.parent.InverseTransformPoint(hitPoint);
        else
            local = hitPoint;

        axisValue = (slideAxis == SlideAxis.X) ? local.x : local.z;
        return true;
    }

    float GetAxisValue(Vector3 localPos)
    {
        return (slideAxis == SlideAxis.X) ? localPos.x : localPos.z;
    }

    Vector3 SetAxisValue(Vector3 localPos, float value)
    {
        if (slideAxis == SlideAxis.X)
            localPos.x = value;
        else
            localPos.z = value;

        return localPos;
    }

    float GetSafeMin()
    {
        return Mathf.Min(minLocalValue, maxLocalValue);
    }

    float GetSafeMax()
    {
        return Mathf.Max(minLocalValue, maxLocalValue);
    }

    public void RefreshEndpointConnections()
    {
        if (allCubes == null || allCubes.Length == 0)
            allCubes = Object.FindObjectsOfType<Cube>();

        RefreshOneEndpoint(frontEndCube);
        RefreshOneEndpoint(backEndCube);
    }

    void RefreshOneEndpoint(Cube endpoint)
    {
        if (endpoint == null) return;

        ClearExternalNeighbors(endpoint);

        Cube target = FindBestExternalNeighbor(endpoint);
        if (target != null)
        {
            AddBidirectionalNeighbor(endpoint, target);
        }
    }

    void ClearExternalNeighbors(Cube endpoint)
    {
        for (int i = endpoint.neighbors.Count - 1; i >= 0; i--)
        {
            Cube other = endpoint.neighbors[i];
            if (other == null)
            {
                endpoint.neighbors.RemoveAt(i);
                continue;
            }

            if (!other.transform.IsChildOf(transform))
            {
                endpoint.neighbors.RemoveAt(i);
                other.neighbors.Remove(endpoint);
            }
        }
    }

    Cube FindBestExternalNeighbor(Cube endpoint)
    {
        Cube best = null;
        float bestDist = Mathf.Infinity;
        Vector3 endpointPos = endpoint.GetTopCenter();

        foreach (var c in allCubes)
        {
            if (c == null) continue;
            if (c == endpoint) continue;
            if (c.transform.IsChildOf(transform)) continue;

            Vector3 otherPos = c.GetTopCenter();

            float yDiff = Mathf.Abs(otherPos.y - endpointPos.y);
            if (yDiff > connectHeightTolerance) continue;

            float dist = Vector3.Distance(endpointPos, otherPos);
            if (dist < connectDistance && dist < bestDist)
            {
                best = c;
                bestDist = dist;
            }
        }

        return best;
    }

    void AddBidirectionalNeighbor(Cube a, Cube b)
    {
        if (a == null || b == null) return;

        if (!a.neighbors.Contains(b))
            a.neighbors.Add(b);

        if (!b.neighbors.Contains(a))
            b.neighbors.Add(a);
    }
}