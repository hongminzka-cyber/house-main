using System.Collections.Generic;
using UnityEngine;

public class FurniturePlacer : MonoBehaviour
{
    public TabManager tabManager;
    public Transform spawnParent;

    [Header("VR 设置")]
    public Transform rightHand;
    public float rayDistance = 10f;
    public LayerMask groundLayer;

    // 👉 每种家具只允许一个
    private Dictionary<GameObject, GameObject> placedObjects = new Dictionary<GameObject, GameObject>();

    // 👉 当前预览
    private GameObject previewObject;
    private GameObject currentPrefab;

    void Update()
    {
        // =========================
        // 0️⃣ 没选家具 → 清除预览
        // =========================
        if (tabManager == null || tabManager.selectedPrefab == null)
        {
            ClearPreview();
            return;
        }

        GameObject prefab = tabManager.selectedPrefab;

        // =========================
        // 1️⃣ 获取目标位置（关键修复）
        // =========================
        if (!TryGetTargetPosition(out Vector3 targetPos))
            return;

        // =========================
        // 2️⃣ 创建/切换预览
        // =========================
        if (previewObject == null || currentPrefab != prefab)
        {
            ClearPreview();

            // 👉 如果已经存在 → 直接用已有物体当“预览”
            if (placedObjects.ContainsKey(prefab))
            {
                previewObject = placedObjects[prefab];
            }
            else
            {
                previewObject = Instantiate(prefab, targetPos, Quaternion.identity);
                SetPreviewMaterial(previewObject);
            }

            currentPrefab = prefab;
        }

        // =========================
        // 3️⃣ 拖动（按住才移动）
        // =========================
        bool isHolding =
            Input.GetMouseButton(0) ||
            Input.GetButton("Fire1");

        bool release =
            Input.GetMouseButtonUp(0) ||
            Input.GetButtonUp("Fire1");

        if (isHolding && previewObject != null)
        {
            previewObject.transform.position = targetPos;
        }

        // =========================
        // 4️⃣ 松手 → 放置
        // =========================
        if (release)
        {
            PlaceOrMove(prefab, targetPos);

            // 👉 放完取消选择（防止一直拖）
            tabManager.selectedPrefab = null;

            ClearPreview();
        }
    }

    // =========================
    // ⭐ 用射线获取位置（VR核心）
    // =========================
    bool TryGetTargetPosition(out Vector3 targetPos)
    {
        targetPos = Vector3.zero;

        bool isVR = rightHand != null && UnityEngine.XR.XRSettings.isDeviceActive;

        if (isVR)
        {
            Ray ray = new Ray(rightHand.position, rightHand.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, groundLayer))
            {
                targetPos = hit.point;
                return true;
            }
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            {
                targetPos = hit.point;
                return true;
            }
        }

        return false;

    }

    // =========================
    // ⭐ 放置 or 移动（核心逻辑）
    // =========================
    void PlaceOrMove(GameObject prefab, Vector3 position)
    {
        GameObject obj;

        // 👉 已存在 → 只移动
        if (placedObjects.ContainsKey(prefab))
        {
            obj = placedObjects[prefab];
            obj.transform.position = position;
        }
        else
        {
            // 👉 第一次才创建
            obj = Instantiate(prefab, position, Quaternion.identity, spawnParent);
            placedObjects.Add(prefab, obj);

            // 👉 只在第一次计数
            FurnitureItem item = obj.GetComponent<FurnitureItem>();
            if (item != null && GameManager.Instance != null)
            {
                GameManager.Instance.RegisterFurniturePlaced(item);
            }
        }
    }

    // =========================
    // 清除预览
    // =========================
    void ClearPreview()
    {
        if (previewObject != null)
        {
            // 👉 如果是临时预览才删除
            if (!placedObjects.ContainsValue(previewObject))
            {
                Destroy(previewObject);
            }

            previewObject = null;
        }
    }

    // =========================
    // 半透明预览
    // =========================
    void SetPreviewMaterial(GameObject obj)
    {
        Renderer[] rends = obj.GetComponentsInChildren<Renderer>();

        foreach (var r in rends)
        {
            if (r.material.HasProperty("_Color"))
            {
                Color c = r.material.color;
                c.a = 0.5f;
                r.material.color = c;
            }
        }
    }
}