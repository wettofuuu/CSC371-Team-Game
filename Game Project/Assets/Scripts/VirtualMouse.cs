using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class VirtualMouse : MonoBehaviour
{
    public GameObject cursor;
    public LevelCompletePopup LevelCompletePopup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        if (Gamepad.current != null){
            cursor.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnClick(InputValue value){
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = cursor.transform.position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.name == "ContinueButton"){
                LevelCompletePopup.Continue();
            } else if (result.gameObject.name == "RestartButton"){
                LevelCompletePopup.RestartLevel();
            }
        }
    }
}
