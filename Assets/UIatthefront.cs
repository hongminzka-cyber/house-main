using UnityEngine;

public class UIFollow : MonoBehaviour
{
    public Transform cameraTransform;
    public float distance = 2f;

    void Update()
    {
        transform.position = cameraTransform.position + cameraTransform.forward * distance;
        transform.LookAt(cameraTransform);
        transform.Rotate(0, 180, 0);
    }
}
