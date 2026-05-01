using UnityEngine;

public class Furniture : MonoBehaviour
{
    private bool isDragging = false;

    public float minX = 0f;
    public float maxX = 12f;
    public float minZ = -26f;
    public float maxZ = -12f;

    void OnMouseDown()
    {
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 targetPos = GetMouseHitPoint();

            // ��סY�ᣨ��ֹ�ɣ�
            targetPos.y = transform.position.y;

            // ���Ʒ�Χ
            float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
            float clampedZ = Mathf.Clamp(targetPos.z, minZ, maxZ);

            transform.position = new Vector3(clampedX, transform.position.y, clampedZ);
        }

        // ��ת
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.Rotate(0, 90, 0);
        }
    }

    Vector3 GetMouseHitPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            // �򵽵������
            if (hit.collider.CompareTag("Ground"))
            {
                return hit.point;
            }
        }

        return transform.position;
    }
}