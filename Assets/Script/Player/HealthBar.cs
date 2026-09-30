using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Referencias")]
    public Health health;
    public Image fillImage;

    void Start()
    {
        if (health == null)
        {
            health = GetComponentInParent<Health>();
        }

        UpdateHealthBar();
    }

    void Update()
    {
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (health == null || fillImage == null)
            return;

        if (health.maxHealth <= 0)
            return;

        float percentage =
            (float)health.currentHealth /
            health.maxHealth;

        fillImage.fillAmount =
            Mathf.Clamp01(percentage);
    }
}