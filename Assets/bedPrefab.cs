using UnityEngine;

public class FurnitureSpawner : MonoBehaviour
{
    public GameObject bedPrefab;

    private GameObject currentFurniture;

    public void SpawnBed()
    {
        if (bedPrefab == null)
        {
            Debug.LogError("bedPrefab没有拖进去！");
            return;
        }

        // 在玩家面前一点点生成
        Vector3 spawnPos = Camera.main.transform.position
                         + Camera.main.transform.forward * 1.5f
                         + Vector3.down * 0.5f;

        currentFurniture = Instantiate(bedPrefab, spawnPos, Quaternion.identity);
    }
}