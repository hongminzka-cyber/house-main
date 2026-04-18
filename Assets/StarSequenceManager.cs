using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSequenceManager : MonoBehaviour
{
    [Header("Stars in order")]
    public StarCollectible star1;
    public StarCollectible star2;
    public StarCollectible star3;

    private int collectedCount = 0;

    void Start()
    {
        collectedCount = 0;

        SetStarVisible(star1, true);
        SetStarVisible(star2, false);
        SetStarVisible(star3, false);
    }

    public void NotifyStarCollected(StarCollectible star)
    {
        if (star == null) return;

        if (star == star1 && collectedCount == 0)
        {
            collectedCount = 1;
            SetStarVisible(star2, true);
            return;
        }

        if (star == star2 && collectedCount == 1)
        {
            collectedCount = 2;
            SetStarVisible(star3, true);
            return;
        }

        if (star == star3 && collectedCount == 2)
        {
            collectedCount = 3;
            Debug.Log("All stars collected!");
        }
    }

    private void SetStarVisible(StarCollectible star, bool visible)
    {
        if (star == null) return;
        star.SetVisible(visible);
    }
}