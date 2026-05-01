using Oculus.Interaction;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroSequenceController : MonoBehaviour
{
    [Header("── 健康提示 ──")]
    [SerializeField] private GameObject healthCanvas;
    [SerializeField] private CanvasGroup healthCanvasGroup;
    [SerializeField] private Button continueButton;
    [SerializeField] private float healthAutoSkipTime = 10f;

    [Header("── Logo ──")]
    [SerializeField] private GameObject logoCanvas;
    [SerializeField] private CanvasGroup logoCanvasGroup;
    [SerializeField] private float logoFadeIn = 1f;
    [SerializeField] private float logoStay = 2f;
    [SerializeField] private float logoFadeOut = 1f;

    [Header("── 房子：桌面小模型 ──")]
    [SerializeField] private GameObject housePrefab;
    [SerializeField] private Transform houseAnchor;
    [SerializeField] private Vector3 houseInitialScale = new Vector3(0.05f, 0.05f, 0.05f);
    [SerializeField] private Vector3 houseFinalScale = new Vector3(8f, 8f, 8f);

    [Header("── 房子：漂浮 ──")]
    [SerializeField] private float floatAmplitude = 0.006f;
    [SerializeField] private float floatSpeed = 1.2f;

    [Header("── 放大进入动画 ──")]
    [SerializeField] private float glowDuration = 1.5f;
    [SerializeField] private float rotateSpeed = 45f;
    [SerializeField] private float scaleDuration = 2.5f;
    [SerializeField] private float scaleFlashProgress = 0.75f;
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private Color glowColor = new Color(1f, 0.8f, 0.4f, 1f);
    [SerializeField] private float glowIntensity = 3f;


    [Header("── Passthrough ──")]
    [SerializeField] private OVRPassthroughLayer passthroughLayer;

    // ── 内部状态 ──────────────────────────────────
    private GameObject _house;
    private bool _houseClicked = false;
    private bool _isFloating = false;
    private Vector3 _floatBasePos;
    private Coroutine _floatCoroutine;

    // ─────────────────────────────────────────────
    private void Start()
    {
        SetPassthrough(false);
        healthCanvas.SetActive(false);
        logoCanvas.SetActive(false);
        StartCoroutine(PlayIntroSequence());
    }

   

    // ══ 主序列 ══════════════════════════════════════
    private IEnumerator PlayIntroSequence()
    {
        yield return null;

        yield return StartCoroutine(Phase1_Health());
        yield return StartCoroutine(Phase2_Logo());
        yield return StartCoroutine(Phase3a_HouseAppear());

        yield return new WaitUntil(() => _houseClicked);

        yield return StartCoroutine(Phase3b_ScaleIntoHouse());
    }

    // ══ Phase 1：健康提示 ════════════════════════════
    private IEnumerator Phase1_Health()
    {
        healthCanvas.SetActive(true);
        yield return StartCoroutine(Fade(healthCanvasGroup, 0f, 1f, 0.5f));

        bool dismissed = false;
        continueButton.onClick.AddListener(() => dismissed = true);

        float t = 0f;
        while (!dismissed && t < healthAutoSkipTime)
        {
            t += Time.deltaTime;

            // A 键跳过
            if (OVRInput.GetDown(OVRInput.Button.One))
                dismissed = true;

            yield return null;
        }

        yield return StartCoroutine(Fade(healthCanvasGroup, 1f, 0f, 0.5f));
        healthCanvas.SetActive(false);
    }

    // ══ Phase 2：Logo ════════════════════════════════
    private IEnumerator Phase2_Logo()
    {
        logoCanvas.SetActive(true);
        yield return StartCoroutine(Fade(logoCanvasGroup, 0f, 1f, logoFadeIn));
        yield return new WaitForSeconds(logoStay);
        yield return StartCoroutine(Fade(logoCanvasGroup, 1f, 0f, logoFadeOut));
        logoCanvas.SetActive(false);
    }

    // ══ Phase 3a：Passthrough 开启，房子弹出在桌面 ═══
    private IEnumerator Phase3a_HouseAppear()

    {

        SetPassthrough(true);

        yield return null;

        _house = Instantiate(housePrefab, houseAnchor.position, houseAnchor.rotation);

        _house.transform.localScale = Vector3.zero;

        // ── 绑定 ISDK 点击事件 ──────────────────────

        var wrapper = _house.GetComponentInChildren<PointableUnityEventWrapper>();

        if (wrapper != null)

        {

            wrapper.WhenSelect.AddListener((_) => OnHouseClicked());

        }

        else

        {

            Debug.LogError("房子 Prefab 上找不到 PointableUnityEventWrapper，请检查 Prefab 配置");

        }

        // 弹出动画

        float elapsed = 0f;

        float popDur = 0.5f;

        while (elapsed < popDur)

        {

            elapsed += Time.deltaTime;

            float s = EaseOutBack(elapsed / popDur);

            _house.transform.localScale = houseInitialScale * s;

            yield return null;

        }

        _house.transform.localScale = houseInitialScale;

        // 开始漂浮，等待点击

        _floatBasePos = houseAnchor.position;

        _isFloating = true;

        _floatCoroutine = StartCoroutine(FloatLoop());

    }


    private IEnumerator FloatLoop()
    {
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime * floatSpeed;
            if (_house != null)
                _house.transform.position =
                    _floatBasePos + new Vector3(0, Mathf.Sin(t) * floatAmplitude, 0);
            yield return null;
        }
    }

    private void OnHouseClicked()
    {
        _houseClicked = true;
        _isFloating = false;

        // 立刻停止漂浮协程，固定位置
        if (_floatCoroutine != null)
        {
            StopCoroutine(_floatCoroutine);
            _floatCoroutine = null;
        }

        // 把房子位置锁定在当前浮动位置
        _floatBasePos = _house.transform.position;
    }

    // ══ Phase 3b：发光 + 旋转 + 放大进入 ════════════
    private IEnumerator Phase3b_ScaleIntoHouse()
    {
        // 停止漂浮，固定位置
        if (_floatCoroutine != null)
            StopCoroutine(_floatCoroutine);
        _house.transform.position = _floatBasePos;

        var renderers = _house.GetComponentsInChildren<MeshRenderer>();

        // ── 阶段一：发光 + 微旋转 ──────────────────
        float elapsed = 0f;
        while (elapsed < glowDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / glowDuration;

            float intensity = Mathf.Lerp(0f, glowIntensity, t);
            SetEmission(renderers, glowColor * intensity);

            _house.transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

            yield return null;
        }

        // ── 阶段二：放大 + 持续旋转 ────────────────
        elapsed = 0f;
        bool fadeFired = false;

        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;

            // EaseInCubic：越来越快，有被吸进去的感觉
            float et = EaseInCubic(t);
            _house.transform.localScale = Vector3.Lerp(houseInitialScale, houseFinalScale, et);

            // 旋转加速
            _house.transform.Rotate(Vector3.up, rotateSpeed * (1f + t * 2f) * Time.deltaTime);

            // 发光随放大增强
            float intensity = Mathf.Lerp(glowIntensity, glowIntensity * 2f, t);
            SetEmission(renderers, glowColor * intensity);

            // 到达指定进度闪黑
            if (!fadeFired && t >= scaleFlashProgress)
            {
                fadeFired = true;
                StartCoroutine(FlashAndLoadScene());
            }

            yield return null;
        }
    }

    private IEnumerator FlashAndLoadScene()
    {
        OVRScreenFade.instance?.FadeOut();
        yield return new WaitForSeconds(flashDuration);
        SceneLoader.Instance?.OnStartButtonPressed();
    }

    // ── Emission 控制 ─────────────────────────────
    private void SetEmission(MeshRenderer[] renderers, Color color)
    {
        foreach (var r in renderers)
        {
            foreach (var mat in r.materials)
            {
                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.SetColor("_EmissionColor", color);
                    mat.EnableKeyword("_EMISSION");
                }
            }
        }
    }

    // ══ 工具方法 ══════════════════════════════════════
    private void SetPassthrough(bool enabled)
    {
        if (passthroughLayer != null)
            passthroughLayer.enabled = enabled;
    }

    private IEnumerator Fade(CanvasGroup cg, float from, float to, float dur)
    {
        float e = 0f;
        cg.alpha = from;
        while (e < dur)
        {
            e += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, e / dur);
            yield return null;
        }
        cg.alpha = to;
    }

    private float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    private float EaseInCubic(float t) =>
        Mathf.Clamp01(t) * Mathf.Clamp01(t) * Mathf.Clamp01(t);
}