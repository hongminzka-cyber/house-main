using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private string targetSceneName = "RoomSceneNew";

    private bool _isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OnStartButtonPressed()
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        SceneManager.LoadScene(targetSceneName);
    }
}