using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public PlayerSystem playerSystem;

    private SliderMechanism activeSlider;

    void Update()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        if (playerSystem != null && playerSystem.IsBusy)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (TryRaycast(cam, out RaycastHit hit))
            {
                Cube cube = hit.collider.GetComponent<Cube>();
                if (cube != null && playerSystem != null)
                {
                    playerSystem.OnCubeClicked(cube);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (TryRaycast(cam, out RaycastHit hit))
            {
                GameObject clickedObj = hit.collider.gameObject;

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
                    activeSlider.StartDragging(hit.point);
                    return;
                }

                RotateMechanism rotator = clickedObj.GetComponentInParent<RotateMechanism>();
                if (rotator != null)
                {
                    rotator.TryRotate();
                    return;
                }
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (activeSlider != null)
            {
                activeSlider.StopDragging();
                activeSlider = null;
            }
        }
    }

    bool TryRaycast(Camera cam, out RaycastHit hit)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit, Mathf.Infinity);
    }
}