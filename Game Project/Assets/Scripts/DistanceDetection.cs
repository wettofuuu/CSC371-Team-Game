using UnityEngine;

public class DistanceDetection : MonoBehaviour
{
    public Transform Player;
    private bool Detected = false;
    public float DetectionRange = 5f;
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

        if (!Detected && Distance <= DetectionRange){
            Detect();
        }
    }

    void Detect(){
        Detected = true;

        for (int i = 0; i < NpcDialogue.DialogueLines.Length; i++){
            DialogueScript.DialogueTrigger(NpcDialogue.DialogueLines[i]);
        }
    }
}
