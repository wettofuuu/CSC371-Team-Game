using UnityEngine;
using System.Collections;

public class KeyPickup : MonoBehaviour
{
    public GameObject keyPickedUI;
    public float showTime = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Movement>() == null) return;

        KeyState.HasKey = true;

        if (keyPickedUI != null)
            StartCoroutine(ShowKeyUI());

        Destroy(gameObject);
    }

    private IEnumerator ShowKeyUI()
    {
        keyPickedUI.SetActive(true);
        yield return new WaitForSeconds(showTime);
        keyPickedUI.SetActive(false);
    }
}