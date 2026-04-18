using System.Collections.Generic;
using UnityEngine;

public class FurniturePlacer : MonoBehaviour
{
    public TabManager tabManager;
    public Transform spawnParent;

    public Transform rightHand;
    public float vrDistance = 1.5f;

    // 🔥 每种家具一个实例
    private Dictionary<GameObject, GameObject> placedObjects = new Dictionary<GameObject, GameObject>();

    void Update()
    {
        if (tabManager == null || tabManager.selectedPrefab == null)
            return;

        GameObject prefab = tabManager.selectedPrefab;

        // =========================
        // 1️⃣ 获取目标位置（PC + VR）
        // =========================
        Vector3 targetPos;

        bool isVR = rightHand != null && UnityEngine.XR.XRSettings.isDeviceActive;

        if (isVR)
        {
            targetPos = rightHand.position + rightHand.forward * vrDistance;
        }
        else
        {
            Vector3 mouse = Input.mousePosition;
            mouse.z = 5f; //主要是这里问题
            targetPos = Camera.main.ScreenToWorldPoint(mouse);
        }

        targetPos.y = spawnParent.position.y;

        // =========================
        // 2️⃣ 如果这个家具已经存在 → 直接移动它
        // =========================
        if (placedObjects.ContainsKey(prefab))
        {
            placedObjects[prefab].transform.position = targetPos;
        }
        else
        {
            // 还没有 → 创建一个
            GameObject obj = Instantiate(prefab, targetPos, Quaternion.identity, spawnParent);
            placedObjects.Add(prefab, obj);
        }

        // =========================
        // 3️⃣ 放置（确认位置）
        // =========================
        bool place =
            Input.GetMouseButtonDown(0) ||
            Input.GetButtonDown("Fire1");

        if (place)
        {
            GameObject obj = placedObjects[prefab];

            obj.transform.position = targetPos;
        }
    }
}