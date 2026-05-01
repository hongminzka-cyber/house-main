using UnityEngine;

public class PlayButtonController : MonoBehaviour
{
    [Header("UI 文本控制")]
    public GameObject textTMP1;     // 对应截图里的 Text (TMP)
    public GameObject textTMP2;     // 对应截图里的 Text (TMP)2

    [Header("音频控制")]
    public AudioSource sound2;      // 对应截图里的 sound2 (Audio Source)
    public AudioSource smAudio;     // 对应截图里的 sm (Audio Source)

    [Header("按钮本体控制")]
    public GameObject playButton;   // 对应截图里的 play (按钮本身)
    public GameObject playButton2;  // 对应截图里的 play2 (另一个按钮)

    // 这个方法就是你按钮要调用的事件
    public void OnPlayClicked()
    {
        // 1. 关掉第一个文本
        if (textTMP1 != null) textTMP1.SetActive(false);
        // 2. 打开第二个文本
        if (textTMP2 != null) textTMP2.SetActive(true);

        // 3. 播放新音乐
        if (sound2 != null) sound2.Play();

        // 4. 暂停旧音乐
        if (smAudio != null) smAudio.Pause();

        // 5. 关掉当前播放按钮 (这句放在最后比较安全)
        if (playButton != null) playButton.SetActive(false);

        // 6. 打开暂停/停止按钮
        if (playButton2 != null) playButton2.SetActive(true);
    }
}