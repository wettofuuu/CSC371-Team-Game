using UnityEngine;
using UnityEngine.InputSystem;

public class WelcomePopup : MonoBehaviour
{   
    private void OnMoveClick(InputValue value){
        if (!value.isPressed) return;

        gameObject.SetActive(false);
    }
}