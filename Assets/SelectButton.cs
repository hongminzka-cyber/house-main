using UnityEngine;

public class TabManager : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject selectedPrefab;

    //private int currentIndex = -1;

    public void Select(int index)
    {
        Debug.Log("BUTTON CLICKED: " + index);

        if (prefabs == null || index < 0 || index >= prefabs.Length)
        {
            Debug.LogWarning("Prefab index invalid");
            return;
        }

        // 👉 点击同一个按钮 = 取消选择
        /* 
        if (currentIndex == index)
        {
            selectedPrefab = null;
            currentIndex = -1;

            Debug.Log("Deselected");
            return;
        }
        */
        selectedPrefab = prefabs[index];
        //currentIndex = index;

        Debug.Log("Selected = " + selectedPrefab.name);
    }
}