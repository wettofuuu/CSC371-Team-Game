using UnityEngine;

public class DontDestroyUI : MonoBehaviour
{
    private static DontDestroyUI instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}