using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KillCount : MonoBehaviour
{
    Combat combat;
    [SerializeField]TextMeshProUGUI killCountText;
    [SerializeField]TextMeshProUGUI killText;

    CanvasGroup canvas;


    private void Start()
    {
        combat = GetComponentInParent<Combat>();
        canvas = GetComponent<CanvasGroup>();

        canvas.alpha = 0f;

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
