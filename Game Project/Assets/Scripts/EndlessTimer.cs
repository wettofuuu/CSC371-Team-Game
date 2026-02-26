using UnityEngine;
using TMPro;

public class EndlessTimer : MonoBehaviour
{   
    public static EndlessTimer Instance;
    public TextMeshProUGUI Timer;

    private float TimePassed = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Awake(){
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update(){
        TimePassed += Time.deltaTime;

        int Minutes = Mathf.FloorToInt(TimePassed / 60f);
        int Seconds = Mathf.FloorToInt(TimePassed % 60f);

        Timer.text = $"{Minutes:00}:{Seconds:00}";
    }
}
