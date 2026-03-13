using UnityEngine;
using UnityEngine.InputSystem;

public class VirtualMouse : MonoBehaviour
{
    public GameObject cursor;
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
}
