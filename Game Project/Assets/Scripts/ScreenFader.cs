using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Canvas fadeCanvas; // optional but recommended
    [SerializeField] private float fadeOutTime = 0.35f;
    [SerializeField] private float fadeInTime = 0.35f;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(transform.root.gameObject); // destroy whole fade canvas if duplicate
            return;
        }
        Instance = this;

        // Keep the whole canvas around across scenes
        DontDestroyOnLoad(transform.root.gameObject);

        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (fadeCanvas == null) fadeCanvas = GetComponentInParent<Canvas>();

        // Ensure it's always on top
        if (fadeCanvas != null)
        {
            fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            fadeCanvas.sortingOrder = 999;
        }
    }

    private void Start()
    {
        // Start black -> fade in
        canvasGroup.alpha = 1f;
        StartCoroutine(Fade(0f, fadeInTime));
    }

    public IEnumerator FadeToScene(string sceneName)
    {
        // Fade out to black
        yield return Fade(1f, fadeOutTime);

        // Load next scene
        yield return SceneManager.LoadSceneAsync(sceneName);

        // Fade in from black
        yield return Fade(0f, fadeInTime);
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float t = 0f;

        canvasGroup.blocksRaycasts = true; // block clicks during fade

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        canvasGroup.blocksRaycasts = targetAlpha > 0.01f;
    }
}
