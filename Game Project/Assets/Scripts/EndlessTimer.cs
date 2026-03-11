using UnityEngine;
using TMPro;

public class EndlessTimer : MonoBehaviour
{
    public static EndlessTimer Instance;
    public TextMeshProUGUI Timer;

    private float TimePassed = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        TimePassed += Time.deltaTime;

        int Minutes = Mathf.FloorToInt(TimePassed / 60f);
        int Seconds = Mathf.FloorToInt(TimePassed % 60f);

        if (Timer != null)
            Timer.text = $"{Minutes:00}:{Seconds:00}";
    }

    public float GetTimeSeconds() => TimePassed;

    public void ResetTimer()
    {
        TimePassed = 0f;
    }
}