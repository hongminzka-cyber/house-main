using UnityEngine;

public class Furniture : MonoBehaviour
{
    private bool isDragging = false;

    public float minX = -5f;
    public float maxX = 5f;
    public float minZ = -5f;
    public float maxZ = 5f;

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

            // 锁住Y轴（防止飞）
            targetPos.y = transform.position.y;

            // 限制范围
            float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
            float clampedZ = Mathf.Clamp(targetPos.z, minZ, maxZ);

            transform.position = new Vector3(clampedX, transform.position.y, clampedZ);
        }

        // 旋转
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
            // 打到地面才用
            if (hit.collider.CompareTag("Ground"))
            {
                return hit.point;
            }
        }

        return transform.position;
    }
}