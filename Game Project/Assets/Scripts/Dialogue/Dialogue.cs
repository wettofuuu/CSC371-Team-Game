using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public GameObject DialogueBox;
    public TextMeshProUGUI DialogueTextbox;
    public NonPlayableMovement Npc;
    private bool Triggered = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        
    }

    private void OnCloseDialogue(InputValue value){
        if (!value.isPressed) return;
        if (!DialogueBox.activeSelf) return;

        DialogueBox.SetActive(false);

        if (Npc != null && !Triggered){
            Triggered = true;
            Npc.MoveBack();
        }
    }

    public void DialogueTrigger(string DialogueText){
        if (Npc != null){
            Npc.Jump();
        }

        DialogueTextbox.text = DialogueText;
        

        DialogueBox.SetActive(true);
    }
}
