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

    public bool IsSpinning => Spinning;   // ✅ add this

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
            Spinning = true;
            StartCoroutine(StopSpinAfterDelay(0.1f));
        }
    }

    void OnMoveClick(InputValue value){
        if (value.isPressed){
            Vector2 MousePos = Mouse.current.position.ReadValue();

            Ray Ray = Camera.ScreenPointToRay(MousePos);
            RaycastHit Hit;

            if (Physics.Raycast(Ray, out Hit, 100f, Ground)){
                Target = Hit.point;
                Target.y = transform.position.y;
                PlayerMoving = true;
            }
        }
    }

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

        if (Vector3.Distance(transform.position, Target) < 0.05f) {
            PlayerMoving = false;
        }
    }
}
