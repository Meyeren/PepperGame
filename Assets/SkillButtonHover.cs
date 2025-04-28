using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class SkillButtonHover : MonoBehaviour, IPointerEnterHandler
{
    [TextArea(3, 10)]
    public string description;
    public TextMeshProUGUI descriptionText;

    public TextMeshProUGUI costText;

    public string cost;

    public void OnPointerEnter(PointerEventData eventData)
    {
            descriptionText.text = description;
            costText.text = "Cost: " + cost;
       
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionText.text = "";
        costText.text = "";
    }
}