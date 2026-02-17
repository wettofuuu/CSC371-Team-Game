using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class Dialogue : MonoBehaviour
{
    public GameObject DialogueBox;
    public NonPlayableMovement Npc;
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

        if (Npc != null){
            Npc.MoveBack();
        }
    }

    public void DialogueTrigger(){
        if (Npc != null){
            Npc.Jump();
        }

        DialogueBox.SetActive(true);
    }
}
