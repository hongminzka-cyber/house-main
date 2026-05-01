using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OVRRigTurnSimple : MonoBehaviour

{
    public Transform rigRoot;        

    public Transform headAnchor;    

    public float turnSpeed = 90f;

    public float deadZone = 0.2f;

    void Update()

    {

        if (rigRoot == null || headAnchor == null)

            return;

        Vector2 stick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);

        float x = stick.x;

        if (Mathf.Abs(x) < deadZone)

            return;

        float yaw = x * turnSpeed * Time.deltaTime;

        rigRoot.RotateAround(headAnchor.position, Vector3.up, yaw);

    }

}
