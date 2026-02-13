using System.Collections;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float SpinSpeed = 1000f;
    private bool Spinning = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private IEnumerator StopSpinAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Spinning = false;
    }

    private float nextActionTime = 0.0f;
    public float period = 5f;
    // Update is called once per frame

    void Update () {

        if (Spinning){
            transform.Rotate(Vector3.up, SpinSpeed * Time.deltaTime);
        }

        if (Time.time > nextActionTime && !Spinning){
            nextActionTime = Time.time + period;
            Spinning = true;
            StartCoroutine(StopSpinAfterDelay(0.2f));
        }
    }
}
