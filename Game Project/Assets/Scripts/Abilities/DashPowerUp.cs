using UnityEngine;
using System.Collections;

public class DashPowerUp : MonoBehaviour
{
    public GameObject UIPopup;
    public float PopupTime = 2f;
    
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip AbilityPickUp;

    private void OnTriggerEnter(Collider other)
    {
        // tries to find Movement on self or parents (robust)
        Movement m = other.GetComponentInParent<Movement>();
        if (m == null) return;

        m.EnableDash();             // unlock dash

        sfxSource.PlayOneShot(AbilityPickUp);

        if (UIPopup != null){
            UIPopup.SetActive(true);
            UIPopup.GetComponent<UIPopup>().Show();
        }

        gameObject.SetActive(false);
    }
}
