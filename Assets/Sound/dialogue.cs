using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueFlowController : MonoBehaviour
{
    public AudioSource audioSource;
    public TextMeshProUGUI dialogueText;

    public GameObject dialogueUI;
    public GameObject furnitureUI;

    public CanvasGroup dialogueCanvas;
    public CanvasGroup furnitureCanvas;

    // ?? 每一段音频
    public AudioClip[] audioClips;

    // ?? 每一段对应的文字（每段可以多句）
    public string[][] dialogueLines;

    public float lineDelay = 2f;
    public float fadeDuration = 1f;

    private int currentStage = 0;

    void Start()
    {
        furnitureUI.SetActive(false);
        StartCoroutine(PlayStage(0));
    }

    IEnumerator PlayStage(int stageIndex)
    {
        currentStage = stageIndex;

        // ?? 淡入对话UI
        dialogueUI.SetActive(true);
        yield return StartCoroutine(FadeIn(dialogueCanvas));

        // ?? 播放对应音频
        audioSource.clip = audioClips[stageIndex];
        audioSource.Play();

        // ?? 播放对应文字
        foreach (string line in dialogueLines[stageIndex])
        {
            dialogueText.text = line;
            yield return new WaitForSeconds(lineDelay);
        }

        // ?? 淡出对话UI
        yield return StartCoroutine(FadeOut(dialogueCanvas));
        dialogueUI.SetActive(false);

        // ?? 根据阶段决定下一步
        if (stageIndex == 0)
        {
            // 第一段 → 第二段
            StartCoroutine(PlayStage(1));
        }
        else if (stageIndex == 1)
        {
            // 第二段 → 家具UI
            ShowFurnitureUI();
        }
        else if (stageIndex == 2)
        {
            // 第三段 → 进入关卡
            LoadNextScene();
        }
    }

    // ?? 显示家具UI
    void ShowFurnitureUI()
    {
        furnitureUI.SetActive(true);
        StartCoroutine(FadeIn(furnitureCanvas));
    }

    // ?? 点击“确认家具”按钮调用这个
    public void OnFurnitureConfirm()
    {
        furnitureUI.SetActive(false);

        // ?? 播放第三段
        StartCoroutine(PlayStage(2));
    }

    // ?? 切场景（你可以改成你的关卡名）
    void LoadNextScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    IEnumerator FadeIn(CanvasGroup cg)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = t / fadeDuration;
            yield return null;
        }
        cg.alpha = 1;
    }

    IEnumerator FadeOut(CanvasGroup cg)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = 1 - (t / fadeDuration);
            yield return null;
        }
        cg.alpha = 0;
    }
}