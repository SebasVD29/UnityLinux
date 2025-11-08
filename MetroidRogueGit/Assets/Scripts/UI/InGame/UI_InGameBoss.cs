using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGameBoss : MonoBehaviour
{
    private Enemy boss;

    [SerializeField] private RectTransform healthRect;
    [SerializeField] private Slider healthSlider;
    //[SerializeField] private TextMeshProUGUI healthText;

    private void Start()
    {
        boss = FindFirstObjectByType<Enemy>();
        boss.health.OnHealthUpdate += UpdateHealthBar;
     
    }

    void UpdateHealthBar()
    {
        float currentHealth = Mathf.RoundToInt(boss.health.GetCurrentHealth());
        float maxHealth = boss.stats.GetTotalHealth();

        //healthText.text = currentHealth + "/" + maxHealth;
        healthSlider.value = boss.health.GetHealthPercent();

    }
}
