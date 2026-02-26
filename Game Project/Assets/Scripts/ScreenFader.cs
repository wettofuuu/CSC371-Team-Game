using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Canvas fadeCanvas;
    [SerializeField] private float fadeOutTime = 0.35f;
    [SerializeField] private float fadeInTime = 0.35f;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(transform.root.gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(transform.root.gameObject);

        if (canvasGroup == null) canvasGroup = GetComponentInChildren<CanvasGroup>();
        if (fadeCanvas == null) fadeCanvas = GetComponentInParent<Canvas>();

        if (fadeCanvas != null)
        {
            fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            fadeCanvas.sortingOrder = 999;
        }
    }

    private void Start()
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = 1f;
        StartCoroutine(Fade(0f, fadeInTime));
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If we're NOT in the middle of a transition, never stay black
        if (!isTransitioning && canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    public IEnumerator FadeToScene(string sceneName)
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        // Fade out
        yield return Fade(1f, fadeOutTime);

        // SAFETY: verify scene is loadable (in Build Settings / correct name)
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"ScreenFader: Cannot load scene '{sceneName}'. " +
                           $"Is it added to Build Settings and spelled exactly right?");
            // Recover: fade back in so you're not stuck on black
            yield return Fade(0f, fadeInTime);
            isTransitioning = false;
            yield break;
        }

        // Load scene
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        if (op == null)
        {
            Debug.LogError($"ScreenFader: LoadSceneAsync returned null for '{sceneName}'.");
            yield return Fade(0f, fadeInTime);
            isTransitioning = false;
            yield break;
        }

        while (!op.isDone) yield return null;

        // Give the new scene 1 frame to initialize cameras/UI
        yield return null;

        // Fade in
        yield return Fade(0f, fadeInTime);

        isTransitioning = false;
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        if (canvasGroup == null) yield break;

        float startAlpha = canvasGroup.alpha;
        float t = 0f;

        canvasGroup.blocksRaycasts = true;

        if (duration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
            canvasGroup.blocksRaycasts = targetAlpha > 0.01f;
            yield break;
        }

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        canvasGroup.blocksRaycasts = targetAlpha > 0.01f;
    }
}