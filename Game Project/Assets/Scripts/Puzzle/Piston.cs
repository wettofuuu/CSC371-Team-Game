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

    private Vector3 startPos;
    private float currentTravel = 0f;

    private bool extending = false;
    private bool retracting = false;
    private float waitTimer;

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

    private NavMeshAgent currentAgent;

    private IEnumerator ReenableAgent(NavMeshAgent Agent, Vector3 NewPosition){
        yield return null; 

        Agent.Warp(NewPosition);
        Agent.enabled = true;
    }

    private void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player")){
            currentAgent = other.GetComponent<NavMeshAgent>();
            if (currentAgent != null){
                currentAgent.enabled = false;
            }
        }
    }

    private void OnTriggerStay(Collider other){
        if (other.CompareTag("Player") && extending){
            Vector3 WorldDirection = transform.TransformDirection(Direction);
            float PushAmount = SlamSpeed * Time.deltaTime;

            other.transform.position += WorldDirection * PushAmount;
        }
    }
}