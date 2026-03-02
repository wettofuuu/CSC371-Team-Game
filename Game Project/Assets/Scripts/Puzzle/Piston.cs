using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Piston : MonoBehaviour
{
    public Vector3 Direction = Vector3.forward;
    public float Distance = 2f;

    public float SlamSpeed = 25f;
    public float RetractSpeed = 20f;
    public float WaitTime = 1f;
    public float PlayerForce = 15f;

    private Vector3 startPos;
    private float currentTravel = 0f;

    private bool extending = false;
    private bool retracting = false;
    private float waitTimer;

    private PlayerKnockback PlayerInside;

    void Start(){
        startPos = transform.localPosition;
        waitTimer = WaitTime;
        Direction = Direction.normalized;
    }

    void Update(){
        if (!extending && !retracting){
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f){
                extending = true;
                TryPushPlayer();
            }
        }

        if (extending){
            float move = SlamSpeed * Time.deltaTime;
            currentTravel += move;

            if (currentTravel >= Distance){
                currentTravel = Distance;
                extending = false;
                retracting = true;
            }

            transform.localPosition = startPos + Direction * currentTravel;
        }

        if (retracting){
            float move = RetractSpeed * Time.deltaTime;
            currentTravel -= move;

            if (currentTravel <= 0f){
                currentTravel = 0f;
                retracting = false;
                waitTimer = WaitTime;
            }

            transform.localPosition = startPos + Direction * currentTravel;
        }
    }

    private void OnTriggerStay(Collider other){
        if (!other.CompareTag("Player")) return;

        PlayerInside = other.GetComponentInParent<PlayerKnockback>();
    }

    private void TryPushPlayer(){   
        if (PlayerInside == null) return;

        Vector3 worldDirection = transform.TransformDirection(Direction);
        PlayerInside.ApplyKnockback(worldDirection * PlayerForce);

        PlayerInside = null;
    }
}