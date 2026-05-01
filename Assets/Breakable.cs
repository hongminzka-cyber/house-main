using UnityEngine;

public class Breakable : MonoBehaviour
{
    public GameObject brokenPrefab; // 碎块

    public void Break()
    {
        // 1️⃣ 生成碎块
        Instantiate(brokenPrefab, transform.position, transform.rotation);

        // 2️⃣ 隐藏原物体
        gameObject.SetActive(false);
    }
}