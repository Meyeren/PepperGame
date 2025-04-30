using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Button))]
public class SkillButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [TextArea(3, 10)]

    public string description;

    public string nameF;

    public TextMeshProUGUI nameText;

    public TextMeshProUGUI descriptionText;

    public TextMeshProUGUI costText;

    public string cost;

    bool isReady;

    PlayerInput input;
    InputAction clickAction;
    GameObject player;

    private bool isControllerMode;
    private Button button;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        input = player.GetComponent<PlayerInput>();
        clickAction = input.actions.FindAction("Jump");

        button = GetComponent<Button>();
        isControllerMode = (Gamepad.current != null);
    }


    private void Update()
    {
        bool currentControllerMode = (Gamepad.current != null);

        if (currentControllerMode != isControllerMode)
        {
            isControllerMode = currentControllerMode;
            if (isControllerMode && EventSystem.current.currentSelectedGameObject == gameObject)
            {
                ShowDescription();
            }
        }

        if (isControllerMode &&
            EventSystem.current.currentSelectedGameObject == gameObject &&
            input.actions["Submit"].WasPressedThisFrame())
        {
            button.onClick.Invoke();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isControllerMode)
        {
            ShowDescription();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isControllerMode)
        {
            HideDescription();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (isControllerMode)
        {
            ShowDescription();
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (isControllerMode)
        {
            HideDescription();
        }
    }

    private void ShowDescription()
    {
        nameText.text = nameF;
        descriptionText.text = description;
        costText.text = cost;
    }

    private void HideDescription()
    {
        nameText.text = "";
        descriptionText.text = "";
        costText.text = "";
    }
}