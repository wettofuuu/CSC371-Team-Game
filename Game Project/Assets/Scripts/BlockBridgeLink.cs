using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

[RequireComponent(typeof(Rigidbody))]
public class BlockBridgeLink : MonoBehaviour
{
    [Header("Assign your BridgeLink NavMeshLink here")]
    public NavMeshLink link;

    [Header("When to activate")]
    public float droppedYDelta = 0.3f;   // how far down from start counts as "in pit"
    public float settleSpeed = 0.05f;    // velocity magnitude considered "still"
    public float settleTime = 0.25f;     // must be still this long

    private Rigidbody rb;
    private float startY;
    private float stillFor = 0f;
    private bool activated = false;

    private NavMeshObstacle obstacle; // optional, but recommended if present

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startY = transform.position.y;

        obstacle = GetComponent<NavMeshObstacle>();

        // IMPORTANT: prefer disabling the component, not the GameObject
        if (link != null)
            link.enabled = false;
    }

    void Update()
    {
        if (activated || link == null) return;

        bool droppedIntoPit = transform.position.y < (startY - droppedYDelta);

        if (rb.linearVelocity.magnitude < settleSpeed) stillFor += Time.deltaTime;
        else stillFor = 0f;

        if (droppedIntoPit && stillFor >= settleTime)
        {
            // Stop carving once it becomes a bridge
            if (obstacle != null) obstacle.enabled = false;

            // Enable the link once
            link.enabled = true;

            activated = true;

            // Stop running Update forever
            enabled = false;
        }
    }
}
