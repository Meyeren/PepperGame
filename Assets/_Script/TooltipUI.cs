using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI instance;
    public GameObject tooltipObject;
    public TextMeshProUGUI tooltipText;

    void Awake()
    {
        instance = this;
        tooltipObject.SetActive(false);
    }

    public static void ShowTooltip(string text)
    {
        instance.tooltipText.text = text;
        instance.tooltipObject.SetActive(true);
    }

    public static void HideTooltip()
    {
        instance.tooltipObject.SetActive(false);
    }
}
