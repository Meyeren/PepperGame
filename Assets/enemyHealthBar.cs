using UnityEngine;
using UnityEngine.UI;

public class enemyHealthBar : MonoBehaviour
{
    EnemyHealth enemyHealthScript;
    CanvasGroup canvas;
    Slider healthBar;


    private void Start()
    {
        enemyHealthScript = GetComponent<EnemyHealth>();
        canvas = GetComponentInChildren<CanvasGroup>();
        healthBar = GetComponentInChildren<Slider>();

        healthBar.maxValue = enemyHealthScript.maxEnemyHealth;
        canvas.alpha = 0f;
    }


    private void Update()
    {
        healthBar.value = enemyHealthScript.enemyHealth;

        if (enemyHealthScript.enemyHealth == enemyHealthScript.maxEnemyHealth)
        {
            FadeOutHealth();
        }
        else
        {
            canvas.alpha = 1.0f;
        }
    }

    void FadeOutHealth()
    {
        if (canvas.alpha > 0f)
        {
            canvas.alpha -= Time.deltaTime * 0.5f;
        }
    }
}
