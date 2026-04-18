using UnityEngine;

public class TabManager : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject selectedPrefab;

    public void Select(int index)
    {
        Debug.Log("BUTTON CLICKED: " + index);

        if (prefabs == null || index < 0 || index >= prefabs.Length)
        {
            Debug.LogWarning("Prefab index invalid");
            return;
        }

        selectedPrefab = prefabs[index];
        Debug.Log("Selected = " + selectedPrefab.name);
    }
}