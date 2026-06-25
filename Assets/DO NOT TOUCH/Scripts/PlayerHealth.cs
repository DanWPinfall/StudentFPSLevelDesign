using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Image healthBar;
    public Image damageBar;

    [Header("Damage Polish")]
    public float damageDelay = 0.15f;
    public float damageDrainSpeed = 1.5f;

    float damageTimer;

    Color normalColour;

    void Start()
    {
        normalColour = healthBar.color;
        currentHealth = maxHealth;

        healthBar.fillAmount = 1f;
        damageBar.fillAmount = 1f;

    }

    void Update()
    {
        if (damageTimer > 0f)
        {
            damageTimer -= Time.deltaTime;
            return;
        }

        if (damageBar.fillAmount > healthBar.fillAmount)
        {
            damageBar.fillAmount = Mathf.MoveTowards(
                damageBar.fillAmount,
                healthBar.fillAmount,
                Time.deltaTime * damageDrainSpeed
            );
        }
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        float normalizedHealth = currentHealth / maxHealth;

        healthBar.fillAmount = normalizedHealth;

        damageTimer = damageDelay;

        healthBar.color = Color.white;
        Invoke(nameof(ResetBarColor), 0.05f);


        OnDamage();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void OnDamage()
    {
        iTween.PunchRotation(
            Camera.main.gameObject,
            new Vector3(2f, Random.Range(-1f, 1f), 0f),
            0.15f
        );
    }

    void ResetBarColor()
    {
        healthBar.color = normalColour;
    }


    void Die()
    {
        SceneManager.LoadScene("SampleScene");
        Debug.Log("Player died");
    }

}
