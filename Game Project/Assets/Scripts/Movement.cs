using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public Camera Camera;
    public float MoveSpeed = 5f;
    public float SpinSpeed = 180f;

    Vector3 Target;
    private bool PlayerMoving;
    private bool Spinning;
    InputAction InputActionSystem;
    public LayerMask Ground;
    
    void Awake(){
        InputActionSystem = InputSystem.actions.FindAction("MoveClick");
    }

    IEnumerator StopSpinAfterDelay(float Delay){
        yield return new WaitForSeconds(Delay);
        Spinning = false;
    }

    void OnSpin(InputValue value){
        if (value.isPressed){
            Spinning = value.isPressed;
            StartCoroutine(StopSpinAfterDelay(0.1f));
        }
    }
    
    void OnMoveClick(InputValue value){
        // when the lft click is pressed, this will fire
        if (value.isPressed){
            Vector2 MousePos = Mouse.current.position.ReadValue();

            Ray Ray = Camera.ScreenPointToRay(MousePos);
            RaycastHit Hit;

            //this is a simple raycast that shoots to mouseposition given ground
            
            if (Physics.Raycast(Ray, out Hit, 100f, Ground)){
                Target = Hit.point;
                Target.y = transform.position.y;
                PlayerMoving = true;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
         if (Spinning){
            transform.Rotate(Vector3.up, SpinSpeed * Time.deltaTime);
        }

        if (!PlayerMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            Target,
            MoveSpeed * Time.deltaTime
        );

        //later we can add pathfinding so its a good path for point and click
        
        if (Vector3.Distance(transform.position, Target) < 0.05f) {
            PlayerMoving = false;
        }
    }
}
