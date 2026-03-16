using UnityEngine;
using UnityEngine.InputSystem;

public class WelcomePopup : MonoBehaviour
{   
    void Update() {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnMoveClick(InputValue value){
        if (!value.isPressed) return;

        gameObject.SetActive(false);
    }
}