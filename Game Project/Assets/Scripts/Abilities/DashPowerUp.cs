using UnityEngine;
using System.Collections;

public class DashPowerUp : MonoBehaviour
{
    public GameObject UIPopup;
    public float PopupTime = 2f;

    private void OnTriggerEnter(Collider other)
    {
        // tries to find Movement on self or parents (robust)
        Movement m = other.GetComponentInParent<Movement>();
        if (m == null) return;

        m.EnableDash();             // unlock dash

        if (UIPopup != null){
            StartCoroutine(ShowPopup());
        }

        gameObject.SetActive(false); // or Destroy(gameObject);
    }

    private IEnumerator ShowPopup(){
        UIPopup.SetActive(true);
        yield return new WaitForSeconds(PopupTime);
        UIPopup.SetActive(false);
    }
}
