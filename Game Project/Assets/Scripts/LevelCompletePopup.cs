using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class LevelCompletePopup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject root;               // the panel (enable/disable)
    [SerializeField] private Image star1;
    [SerializeField] private Image star2;
    [SerializeField] private Image star3;
    [SerializeField] private Sprite starOn;
    [SerializeField] private Sprite starOff;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text livesText;

    [Header("Audio")]
    [SerializeField] private AudioSource completeSound;

    [Header("Level Flow")]
    [Tooltip("If empty, Continue will load the next build index scene.")]
    [SerializeField] private string nextSceneName = "";

    [Header("3-Star Requirements (edit per level)")]
    [Tooltip("3 stars if time <= this AND lives >= livesFor3Stars")]
    [SerializeField] private float timeFor3Stars = 45f;
    [SerializeField] private int livesFor3Stars = 7;

    [Header("2-Star Requirements (edit per level)")]
    [Tooltip("2 stars if time <= this AND lives >= livesFor2Stars")]
    [SerializeField] private float timeFor2Stars = 75f;
    [SerializeField] private int livesFor2Stars = 4;

    private bool shown = false;

    public void SetNextScene(string scene)
    {
        nextSceneName = scene;
    }

    private void Awake()
    {
        if (root == null) root = gameObject;
        root.SetActive(false);
    }

    // Call this when the player finishes the level
    public void Show()
    {
        if (shown) return;
        shown = true;

        root.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 🔊 Play completion sound
        if (completeSound != null)
            completeSound.Play();

        float t = (EndlessTimer.Instance != null) ? EndlessTimer.Instance.GetTimeSeconds() : 0f;
        int lives = Movement.Lives;

        timeText.text = $"Time: {FormatTime(t)}";
        livesText.text = $"Lives: {lives}";

        int stars = CalculateStars(t, lives);
        SetStars(stars);
    }

    public void Hide()
    {
        root.SetActive(false);
        Time.timeScale = 1f;
        shown = false;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        // optional: reset timer on restart
        if (EndlessTimer.Instance != null)
            EndlessTimer.Instance.ResetTimer();

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    public void Continue()
    {
        Time.timeScale = 1f;

        // ✅ reset timer when continuing
        if (EndlessTimer.Instance != null)
            EndlessTimer.Instance.ResetTimer();

        Debug.Log($"[Continue] nextSceneName='{nextSceneName}'");

        bool inBuild = Enumerable.Range(0, SceneManager.sceneCountInBuildSettings)
            .Select(SceneUtility.GetScenePathByBuildIndex)
            .Any(p => System.IO.Path.GetFileNameWithoutExtension(p) == nextSceneName);

        Debug.Log($"[Continue] inBuild={inBuild}");

        if (!inBuild)
        {
            Debug.LogError($"Scene '{nextSceneName}' is NOT in Build Settings!");
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    private int CalculateStars(float timeSeconds, int lives)
    {
        // 3 stars
        if (timeSeconds <= timeFor3Stars && lives >= livesFor3Stars)
            return 3;

        // 2 stars
        if (timeSeconds <= timeFor2Stars && lives >= livesFor2Stars)
            return 2;

        // otherwise 1 star for completing
        return 1;
    }

    private void SetStars(int stars)
    {
        if (star1 != null) star1.sprite = (stars >= 1) ? starOn : starOff;
        if (star2 != null) star2.sprite = (stars >= 2) ? starOn : starOff;
        if (star3 != null) star3.sprite = (stars >= 3) ? starOn : starOff;
    }

    private string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        return $"{m:00}:{s:00}";
    }
}