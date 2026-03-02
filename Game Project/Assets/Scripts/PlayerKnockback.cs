using UnityEngine;
using System.Collections;

public class PlayerKnockback : MonoBehaviour
{
    public float RecoveryTime = 1.5f;

    private UnityEngine.AI.NavMeshAgent Agent;
    private Rigidbody Rb;

    private bool IsKnocked = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake(){
        Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Rb = GetComponent<Rigidbody>();
    }

    public void ApplyKnockback(Vector3 force){
        if (!IsKnocked){
            StartCoroutine(KnockbackRoutine());
        }

        Rb.AddForce(force, ForceMode.Impulse);
    }

    private IEnumerator KnockbackRoutine(){
        IsKnocked = true;

        Agent.enabled = false;

        Rb.isKinematic = false;
        Rb.useGravity = true;

        yield return new WaitForSeconds(RecoveryTime);

        Rb.linearVelocity = Vector3.zero;
        Rb.angularVelocity = Vector3.zero;
        Rb.isKinematic = true;

        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out hit, 3f, UnityEngine.AI.NavMesh.AllAreas)){
            Agent.enabled = true;
            Agent.Warp(hit.position);
        } else {
            Agent.enabled = true;
        }

        IsKnocked = false;
    }
}
