using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KillCount : MonoBehaviour
{
    Combat combat;
    [SerializeField]TextMeshProUGUI killCountText;
    [SerializeField]TextMeshProUGUI killText;

    CanvasGroup canvas;

    int lastKillCount;
    float checkTimer = 0f; 

    private void Start()
    {
        combat = GetComponentInParent<Combat>();
        canvas = GetComponent<CanvasGroup>();
        lastKillCount = combat.killCount;
        canvas.alpha = 0f;
        checkTimer = 6f;
    }

    private void Update()
    {
        killCountText.text = combat.killCount.ToString();
        if (combat.killCount == 0)
        {
            canvas.alpha = 0f;
        }
        else
        {
            canvas.alpha = 1f;
        }

        if (combat.isControl)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
        
    }
}
