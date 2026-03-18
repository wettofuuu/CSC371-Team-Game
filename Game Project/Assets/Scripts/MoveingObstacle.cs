using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    public Vector3 direction = Vector3.right; // direction of movement
    public float distance = 3f;               // how far it moves
    public float speed = 2f;                  // movement speed

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float movement = Mathf.Sin(Time.time * speed) * distance;
        transform.position = startPosition + direction.normalized * movement;
    }
}
