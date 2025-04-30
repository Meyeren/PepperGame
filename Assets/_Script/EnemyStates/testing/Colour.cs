using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class SelectableDisabledButton : MonoBehaviour
{
    private Button button;
    private RawImage rawImage; // Changed from Image to RawImage

    [Header("Selection Override")]
    public Color selectedTintColor = Color.yellow;
    public float selectedTintIntensity = 0.5f; // How strong the tint appears

    private Color normalColor;
    private Color disabledColor;

    void Awake()
    {
        button = GetComponent<Button>();
        rawImage = GetComponent<RawImage>(); // Get RawImage instead

        // Store original colors
        normalColor = rawImage.color;
        disabledColor = button.colors.disabledColor;
    }

    void Update()
    {
        if (EventSystem.current == null) return;

        bool isSelected = (EventSystem.current.currentSelectedGameObject == gameObject);

        if (!button.interactable)
        {
            if (isSelected)
            {
                // Apply tint overlay while keeping disabled appearance
                rawImage.color = Color.Lerp(disabledColor, selectedTintColor, selectedTintIntensity);
            }
            else
            {
                rawImage.color = disabledColor;
            }
        }
        else
        {
            rawImage.color = normalColor;
        }
    }
}
