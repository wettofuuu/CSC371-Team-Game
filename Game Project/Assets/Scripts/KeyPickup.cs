using UnityEngine;
using System.Collections;

public class KeyPickup : MonoBehaviour
{
    public GameObject keyPickedUI;
    public float showTime = 2f;

    private Coroutine uiRoutine;
    private bool pickedUp;

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return; // prevent double-trigger
        if (other.GetComponentInParent<Movement>() == null) return;

        pickedUp = true;

        KeyState.HasKey = true;

        // Stop the key from being triggered again / seen again
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        // Show UI + hide after time, THEN destroy key
        if (keyPickedUI != null)
        {
            if (uiRoutine != null) StopCoroutine(uiRoutine);
            uiRoutine = StartCoroutine(ShowKeyUIThenDestroy());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator ShowKeyUIThenDestroy()
    {
        keyPickedUI.SetActive(true);

        // Realtime so it still works while paused (Time.timeScale = 0)
        yield return new WaitForSecondsRealtime(showTime);

        keyPickedUI.SetActive(false);

        Destroy(gameObject);
    }
}