using UnityEngine;
using UnityEngine.AI;

public class obstacle : MonoBehaviour
{
    public string requiredTag = "BridgeBlock";

    private NavMeshObstacle ob;
    private int objectsInside = 0;

    void Awake()
    {
        ob = GetComponent<NavMeshObstacle>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(requiredTag))
        {
            objectsInside++;
            ob.enabled = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(requiredTag))
        {
            objectsInside--;

            if (objectsInside <= 0)
            {
                ob.enabled = true;
                objectsInside = 0;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
