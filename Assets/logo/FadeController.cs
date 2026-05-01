using System.Collections;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance { get; private set; }

    [SerializeField] private OVRScreenFade ovrFade;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator FadeOut(float duration)
    {
        if (ovrFade != null)
            ovrFade.FadeOut();
        yield return new WaitForSeconds(duration);
    }

    public IEnumerator FadeIn(float duration)
    {
        if (ovrFade != null)
            ovrFade.FadeIn();
        yield return new WaitForSeconds(duration);
    }
}