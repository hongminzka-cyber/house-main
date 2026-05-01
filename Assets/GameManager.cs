using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int placedCount = 0;
    public int targetCount = 3;

    public Button confirmButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (confirmButton != null)
            confirmButton.interactable = false;
    }

    public void RegisterFurniturePlaced(FurnitureItem item)
    {
        if (item.isRegistered) return;

        item.isRegistered = true;
        placedCount++;

        Debug.Log("已放置家具: " + placedCount);

        if (placedCount >= targetCount && confirmButton != null)
        {
            confirmButton.interactable = true;
        }
    }

    public void OnConfirmClicked()
    {
        Breakable[] all = FindObjectsOfType<Breakable>();

        foreach (Breakable b in all)
        {
            b.Break();
        }
    }
}