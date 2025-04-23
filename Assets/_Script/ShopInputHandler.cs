using UnityEngine;
using UnityEngine.InputSystem;

public class ShopInputHandler : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private Vector2 navigateInput;
    private bool submitPressed;

    public Vector2 Navigate => navigateInput;
    public bool SubmitPressed => submitPressed;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.UI.Navigate.performed += ctx => navigateInput = ctx.ReadValue<Vector2>();
        inputActions.UI.Navigate.canceled += ctx => navigateInput = Vector2.zero;

        inputActions.UI.Submit.performed += ctx => submitPressed = true;
    }

    void OnEnable()
    {
        inputActions.UI.Enable();
    }

    void OnDisable()
    {
        inputActions.UI.Disable();
    }

    void LateUpdate()
    {
        submitPressed = false; // reset hver frame
    }
}
