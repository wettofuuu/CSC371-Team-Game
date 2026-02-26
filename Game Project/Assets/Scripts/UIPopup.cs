using UnityEngine;
using System.Collections;

public class UIPopup : MonoBehaviour
{
    public float DisplayTime = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(){
        StopAllCoroutines();
        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine(){
        gameObject.SetActive(true);
        yield return new WaitForSeconds(DisplayTime);
        gameObject.SetActive(false);
    }
}
