using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SkillButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(3, 10)]
    public string description;
    public TextMeshProUGUI descriptionText;

    public TextMeshProUGUI costText;

    public string cost;

    bool isReady;

    PlayerInput input;
    InputAction clickAction;
    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        input = player.GetComponent<PlayerInput>();
        clickAction = input.actions.FindAction("Jump");
    }


    private void Update()
    {
        if (clickAction.triggered && isReady)
        {
            GetComponent<Button>().onClick.Invoke();
            isReady = false;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
            descriptionText.text = description;
            costText.text = "Cost: " + cost;
            isReady = true;

            
       
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Exit");
        isReady = false;
    }
}