using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSystem : MonoBehaviour
{
    [Header("References")]
    public PlayerSystem playerSystem;

    [Header("Controller Ray Origins")]
    public Transform leftRayOrigin;   // 左手控制器射线原点
    public Transform rightRayOrigin;  // 右手控制器射线原点

    [Header("Raycast")]
    public float maxRayDistance = 20f;
    public LayerMask interactionMask = ~0;

    [Header("Debug")]
    public bool showDebugLog = false;

    private SliderMechanism activeSlider;

    void Update()
    {
        OVRInput.Update();

        // A 松开：结束滑块拖动
        if (activeSlider != null && OVRInput.GetUp(OVRInput.Button.One))
        {
            activeSlider.StopDragging();
            activeSlider = null;
            return;
        }

        // 正在拖滑块时，不处理别的交互
        if (activeSlider != null)
            return;

        if (playerSystem != null && playerSystem.IsBusy)
            return;

        // X 按钮：走路
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            HandleMoveClick();
        }

        // A 按钮：机关
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            HandleMechanismClick();
        }
    }

    void HandleMoveClick()
    {
        if (!TryControllerRaycast(leftRayOrigin, out RaycastHit hit))
            return;

        GameObject clickedObj = hit.collider.gameObject;

        if (showDebugLog)
            Debug.Log("[VR Move / X] Hit: " + clickedObj.name);

        Cube cube = clickedObj.GetComponent<Cube>();
        if (cube != null && playerSystem != null)
        {
            playerSystem.OnCubeClicked(cube);
        }
    }

    void HandleMechanismClick()
    {
        if (!TryControllerRaycast(rightRayOrigin, out RaycastHit hit))
            return;

        GameObject clickedObj = hit.collider.gameObject;

        if (showDebugLog)
            Debug.Log("[VR Mechanism / A] Hit: " + clickedObj.name);

        // 1. 墙跳按钮
        WallJumpMechanism jumpMechanism = clickedObj.GetComponentInParent<WallJumpMechanism>();
        if (jumpMechanism != null)
        {
            jumpMechanism.TryActivate();
            return;
        }

        // 2. 滑块机关：开始拖动
        SliderMechanism slider = clickedObj.GetComponentInParent<SliderMechanism>();
        if (slider != null)
        {
            activeSlider = slider;
            activeSlider.StartDragging(rightRayOrigin);
            return;
        }

        // 3. 旋转机关
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
                Debug.LogWarning("Controller ray origin is not assigned.");
            return false;
        }

        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        return Physics.Raycast(ray, out hit, maxRayDistance, interactionMask, QueryTriggerInteraction.Ignore);
    }
}