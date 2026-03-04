using UnityEngine;

public class Float : MonoBehaviour
{
    public float floatSpeed = 2f;      // How fast it floats
    public float floatHeight = 0.5f;   // How high it floats

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}