using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject conButton;
    [SerializeField] GameObject quitButton;
    [SerializeField] TextMeshProUGUI Header;
    [SerializeField] GameObject FOVslider;
    [SerializeField] GameObject SensSlider;
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject settingsButton;

    PlayerInput input;
    InputAction pauseAction;
    InputAction pauseActionUI;
    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;

    GameObject player;
    Combat combat;
    skillTreeManager manager;
    [SerializeField]GameObject skillTree;

    [SerializeField]bool isOpen;

    private void Start()
    {
        
        player = GameObject.FindGameObjectWithTag("Player");
        combat = player.GetComponent<Combat>();
        manager = GameObject.Find("SkillManager").GetComponent<skillTreeManager>();

        input = player.GetComponent<PlayerInput>();
        pauseAction = input.actions.FindActionMap("Player").FindAction("Pause");
        pauseActionUI = input.actions.FindActionMap("UI").FindAction("PauseUI");

        playerActionMap = input.actions.FindActionMap("Player");
        uiActionMap = input.actions.FindActionMap("UI");

        if (Gamepad.current != null)
        {
            SensSlider.GetComponent<Slider>().value = player.GetComponent<PlayerMovement>().sensitivity + 1;
        }
        else
        {
            SensSlider.GetComponent<Slider>().value = player.GetComponent<PlayerMovement>().sensitivity;
        }


        FOVslider.GetComponent<Slider>().value = Camera.main.fieldOfView;

        canvas.SetActive(false);

        
    }

    private void Update()
    {
        player.GetComponent<PlayerMovement>().sensitivity = SensSlider.GetComponent<Slider>().value;
        Camera.main.fieldOfView = FOVslider.GetComponent<Slider>().value;
        player.GetComponent<PlayerMovement>().FOV = FOVslider.GetComponent<Slider>().value;
        if (pauseActionUI.triggered && isOpen)
        {
            Time.timeScale = 1f;
            skillTree.SetActive(true);
            isOpen = false;
            canvas.SetActive(false);
            uiActionMap.Disable();
            if (Gamepad.current != null)
            {
                playerActionMap.Enable();
                
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        if (pauseAction.triggered && !isOpen && !combat.DeathScreen.activeInHierarchy && !manager.isSkilltreeOpen)
        {
            Header.text = "PAUSED";
            conButton.SetActive(true);
            quitButton.SetActive(true);
            FOVslider.SetActive(false);
            SensSlider.SetActive(false);
            backButton.SetActive(false);
            settingsButton.SetActive(true);

            if (Gamepad.current != null) {
                Gamepad.current.SetMotorSpeeds(0f, 0f);
            }
            

            Time.timeScale = 0f;
            skillTree.SetActive(false);
            isOpen = true;
            canvas.SetActive(true);
            uiActionMap.Enable();
            if (Gamepad.current != null)
            {
                playerActionMap.Disable();
                
                EventSystem.current.SetSelectedGameObject(conButton);
                

            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

        }

    }

    public void Continue()
    {
        Time.timeScale = 1f;
        skillTree.SetActive(true);
        isOpen = false;
        canvas.SetActive(false);
        uiActionMap.Disable();
        if (Gamepad.current != null)
        {
            playerActionMap.Enable();
            
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Settings()
    {
        if (Gamepad.current != null)
        {
            EventSystem.current.SetSelectedGameObject(backButton);


        }
        Header.text = "SETTINGS";
        conButton.SetActive(false);
        settingsButton.SetActive(false);
        quitButton.SetActive(false);
        FOVslider.SetActive(true);
        SensSlider.SetActive(true);
        backButton.SetActive(true);
    }

    public void Back()
    {
        if (Gamepad.current != null)
        {
            EventSystem.current.SetSelectedGameObject(conButton);


        }
        Header.text = "PAUSED";
        settingsButton.SetActive(true);
        conButton.SetActive(true);
        quitButton.SetActive(true);
        FOVslider.SetActive(false);
        SensSlider.SetActive(false);
        backButton.SetActive(false);
    }


}
