using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSystem : MonoBehaviour

{

    [Header("References")]

    public PlayerSystem playerSystem;

    [Header("Controller Ray Origins")]

    public Transform leftRayOrigin;

    public Transform rightRayOrigin;

    [Header("Raycast")]

    public float maxRayDistance = 20f;

    public LayerMask interactionMask = ~0;

    [Header("Buttons")]

    public OVRInput.Button moveButton = OVRInput.Button.Three;      

    public OVRInput.Button mechanismButton = OVRInput.Button.One;   

    [Header("Visual Rays")]

    public bool showVisibleRays = true;

    public LineRenderer leftLine;

    public LineRenderer rightLine;

    public float rayWidth = 0.02f;

    public Color leftRayColor = Color.cyan;

    public Color rightRayColor = Color.yellow;

    [Header("Debug")]

    public bool showDebugLog = true;

    private SliderMechanism activeSlider;

    private bool lastMoveButtonState = false;

    private bool lastMechanismButtonState = false;

    void Start()

    {

        SetupLineRenderer(ref leftLine, "LeftRayLine", leftRayColor);

        SetupLineRenderer(ref rightLine, "RightRayLine", rightRayColor);

        if (showDebugLog)

        {

            Debug.Log("ClickSystem Start()");

            Debug.Log("Left Ray Origin = " + (leftRayOrigin != null ? leftRayOrigin.name : "NULL"));

            Debug.Log("Right Ray Origin = " + (rightRayOrigin != null ? rightRayOrigin.name : "NULL"));

        }

    }

    void Update()

    {

        OVRInput.Update();

        UpdateVisibleRay(leftRayOrigin, leftLine);

        UpdateVisibleRay(rightRayOrigin, rightLine);

        bool movePressedNow = OVRInput.Get(moveButton);

        bool mechanismPressedNow = OVRInput.Get(mechanismButton);

        bool movePressedThisFrame = movePressedNow && !lastMoveButtonState;

        bool mechanismPressedThisFrame = mechanismPressedNow && !lastMechanismButtonState;

        if (activeSlider != null)

        {

            if (mechanismPressedThisFrame)

            {

                activeSlider.StopDragging();

                activeSlider = null;

            }

            lastMoveButtonState = movePressedNow;

            lastMechanismButtonState = mechanismPressedNow;

            return;

        }

        if (playerSystem != null && playerSystem.IsBusy)

        {

            lastMoveButtonState = movePressedNow;

            lastMechanismButtonState = mechanismPressedNow;

            return;

        }

        if (movePressedThisFrame)

        {

            if (showDebugLog) Debug.Log("[VR Move] Button pressed");

            HandleMoveClick();

        }

        if (mechanismPressedThisFrame)

        {

            if (showDebugLog) Debug.Log("[VR Mechanism] Button pressed");

            HandleMechanismClick();

        }

        lastMoveButtonState = movePressedNow;

        lastMechanismButtonState = mechanismPressedNow;

    }

    void HandleMoveClick()

    {

        if (!TryControllerRaycast(leftRayOrigin, out RaycastHit hit))

        {

            if (showDebugLog) Debug.Log("[VR Move] No hit");

            return;

        }

        GameObject clickedObj = hit.collider.gameObject;

        if (showDebugLog)

            Debug.Log("[VR Move] Hit: " + clickedObj.name);

        Cube cube = clickedObj.GetComponentInParent<Cube>();

        if (cube != null && playerSystem != null)

        {

            playerSystem.OnCubeClicked(cube);

        }

    }

    void HandleMechanismClick()

    {

        if (!TryControllerRaycast(rightRayOrigin, out RaycastHit hit))

        {

            if (showDebugLog) Debug.Log("[VR Mechanism] No hit");

            return;

        }

        GameObject clickedObj = hit.collider.gameObject;

        if (showDebugLog)

            Debug.Log("[VR Mechanism] Hit: " + clickedObj.name);

        WallJumpMechanism jumpMechanism = clickedObj.GetComponentInParent<WallJumpMechanism>();

        if (jumpMechanism != null)

        {

            jumpMechanism.TryActivate();

            return;

        }

        SliderMechanism slider = clickedObj.GetComponentInParent<SliderMechanism>();

        if (slider != null)

        {

            activeSlider = slider;

            activeSlider.StartDragging(rightRayOrigin);

            if (showDebugLog)

                Debug.Log("[VR Mechanism] Start slider control");

            return;

        }

        RotateMechanism rotator = clickedObj.GetComponentInParent<RotateMechanism>();

        if (rotator != null)

        {

            rotator.TryRotate();

            return;

        }

    }

    bool TryControllerRaycast(Transform rayOrigin, out RaycastHit hit)

    {

        hit = default;

        if (rayOrigin == null)

        {

            if (showDebugLog)

                Debug.LogWarning("Ray origin is not assigned.");

            return false;

        }

        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        return Physics.Raycast(ray, out hit, maxRayDistance, interactionMask, QueryTriggerInteraction.Ignore);

    }

    void UpdateVisibleRay(Transform rayOrigin, LineRenderer line)

    {

        if (!showVisibleRays || rayOrigin == null || line == null)

            return;

        Vector3 start = rayOrigin.position;

        Vector3 end = start + rayOrigin.forward * maxRayDistance;

        if (Physics.Raycast(start, rayOrigin.forward, out RaycastHit hit, maxRayDistance, interactionMask, QueryTriggerInteraction.Ignore))

        {

            end = hit.point;

        }

        line.enabled = true;

        line.SetPosition(0, start);

        line.SetPosition(1, end);

    }

    void SetupLineRenderer(ref LineRenderer line, string objectName, Color color)

    {

        if (line == null)

        {

            GameObject go = new GameObject(objectName);

            go.transform.SetParent(transform);

            line = go.AddComponent<LineRenderer>();

        }

        line.material = new Material(Shader.Find("Sprites/Default"));

        line.startWidth = rayWidth;

        line.endWidth = rayWidth;

        line.positionCount = 2;

        line.useWorldSpace = true;

        line.startColor = color;

        line.endColor = color;

        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        line.receiveShadows = false;

        line.enabled = showVisibleRays;

    }

}
