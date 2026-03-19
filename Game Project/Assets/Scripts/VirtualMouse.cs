using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class VirtualMouse : MonoBehaviour
{
    public GameObject cursor;
    public LevelCompletePopup LevelCompletePopup;
    public MainMenuController MainMenuController;
    public PauseMenu PauseMenu;
    public StoryUIController StoryUIController;
    
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
            } else if (result.gameObject.name == "PlayButton"){
                MainMenuController.Play();
            } else if (result.gameObject.name == "QuitButton"){
                MainMenuController.Quit();
            } else if (result.gameObject.name == "Level 1"){
                PauseMenu.Level1();
            } else if (result.gameObject.name == "Level 2"){
                PauseMenu.Level2();
            } else if (result.gameObject.name == "Level 3"){
                PauseMenu.Level3();
            } else if (result.gameObject.name == "Level 4"){
                PauseMenu.Level4();
            } else if (result.gameObject.name == "Level 5"){
                PauseMenu.Level5();
            } else if (result.gameObject.name == "Resume Button"){
                PauseMenu.ResumeButton();
            } else if (result.gameObject.name == "Main Menu"){
                PauseMenu.MainMenuButton();
            } else if (result.gameObject.name == "StoryButton"){
                MainMenuController.Story();
            } else if (result.gameObject.name == "BackButton"){
                StoryUIController.PreviousPage();
            } else if (result.gameObject.name == "NextButton"){
                StoryUIController.NextPage();
            } else if (result.gameObject.name == "ReturnButton"){
                StoryUIController.ReturnToMainMenu();
            }
        }
    }
}
