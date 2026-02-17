using UnityEngine;

public class NonPlayableMovement : MonoBehaviour
{
    public Transform NonPlayableObject;
    public Rigidbody Rb;
    public float JumpForce = 5f;
    public float MoveSpeed = 3f;

    void Start(){
        if (Rb == null){
            Rb = GetComponent<Rigidbody>();
        }
    }

    public void Jump(){
        Rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    }

    public void MoveBack(){
        transform.position -= transform.forward * MoveSpeed;
    }
}
