using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] private Image healthFill;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetHealth(float current, float max)
    {
        if (healthFill == null || max <= 0f) return;
        healthFill.fillAmount = Mathf.Clamp01(current / max);
    }
}