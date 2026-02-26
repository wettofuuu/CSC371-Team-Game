using UnityEngine;

public class DistanceDetection : MonoBehaviour
{
    [Header("Audio")]
[SerializeField] private AudioSource npcAudio;
[SerializeField] private AudioClip detectClip;
    public Transform Player;
    private bool Detected = false;
    public float DetectionRange = 5f;
    private float ExitRange = 5f;
    public float IdealExitRange = 3f;
    public bool ChangeRange = true;
    public DialogueData NpcDialogue;

    public Dialogue DialogueScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float Distance = Vector3.Distance(transform.position, Player.position);

        if (!Detected && Distance <= DetectionRange)
        {
            Detected = true;
            if (npcAudio != null && detectClip != null)
                npcAudio.PlayOneShot(detectClip);

            if (ChangeRange){
                DetectionRange = 2f;
                ExitRange = IdealExitRange;
            }

            DialogueScript.DialogueTrigger(NpcDialogue.DialogueLines[0]);
        }

        if (Detected && Distance > ExitRange && !DialogueScript.DialogueBox.activeSelf)
        {
            Detected = false;
            DialogueScript.DialogueBox.SetActive(false);
        }
    }
}
