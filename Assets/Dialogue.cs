using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public Text dialogueText;
    public CanvasGroup dialogueUI;

    public AudioSource audioSource;
    public AudioClip[] clips;

    [TextArea]
    public string[] sentences;

    public float fadeSpeed = 2f;

    void Start()
    {
        StartCoroutine(PlayDialogue());
    }

    IEnumerator PlayDialogue()
    {
        yield return StartCoroutine(FadeIn());

        for (int i = 0; i < sentences.Length; i++)
        {
            dialogueText.text = sentences[i];

            audioSource.clip = clips[i];
            audioSource.Play();

            yield return new WaitForSeconds(clips[i].length + 0.5f);
        }

        yield return StartCoroutine(FadeOut());

        // 播完进入家具UI
        FindObjectOfType<FurnitureUI>().ShowUI();
    }

    IEnumerator FadeIn()
    {
        while (dialogueUI.alpha < 1)
        {
            dialogueUI.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        while (dialogueUI.alpha > 0)
        {
            dialogueUI.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
}