using System.Collections.Generic;
using UnityEngine;

public class FurnitureSpawnManager : MonoBehaviour
{
    public Transform spawnPoint;

    // 🔥 记录已经生成的家具
    private Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

    public void Spawn(GameObject prefab)
    {
        if (prefab == null) return;

        // ✅ 如果已经生成过 → 不再创建
        if (spawnedObjects.ContainsKey(prefab))
        {
            Debug.Log("已经生成过这个家具了");
            return;
        }

        GameObject obj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        spawnedObjects.Add(prefab, obj);
    }
}