using UnityEngine;

public class DontDestroyMusic : MonoBehaviour
{
    private static DontDestroyMusic instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}