using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRRigForwardMove : MonoBehaviour
{
    [Header("References")]
    public Transform rigRoot;      
    public Transform headAnchor;  
    [Header("Move Settings")]
    public float moveSpeed = 2f;
    public float deadZone = 0.2f;
    [Header("Input")]
    public OVRInput.Controller moveController = OVRInput.Controller.RTouch;
    void Update()
    {
        if (rigRoot == null || headAnchor == null)
            return;
        Vector2 stick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, moveController);
        float y = stick.y;
        if (Mathf.Abs(y) < deadZone)
            return;
        Vector3 forward = Vector3.ProjectOnPlane(headAnchor.forward, Vector3.up).normalized;
        if (forward.sqrMagnitude < 0.0001f)
            return;
        Vector3 move = forward * y * moveSpeed * Time.deltaTime;
        rigRoot.position += move;
    }
}