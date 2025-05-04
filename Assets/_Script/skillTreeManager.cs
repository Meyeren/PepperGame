using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using TMPro.Examples;
using UnityEngine.EventSystems;
using System;

public class skillTreeManager : MonoBehaviour
{
    PlayerInput input;
    //InputAction naviInput;
    InputAction interactAction;

    GameObject player;
    GameObject target;

    PlayerClass playerClass;

    [Header("Player")]
    public bool hasClass;
    public bool openSkillTree;
    public int skillPoint;

    [Header("UI")]
    public Canvas skillTree;
    public TextMeshProUGUI skillTreeText;
    public TextMeshProUGUI skillPointText;
    public GameObject NormalObject;
    public EventSystem eventSystem;



    [Header("Arrays")]
    public Button[] Runner;
    public Button[] Warrior;
    public Button[] Tank;
    public Button Normal;
    public Button Reset;


    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;
    private int skillPointsSpend;
    private GameObject lastSelectedButton;
    InputAction Interact;

    public bool skillTreeHasBeenOpened;

    public bool isSkilltreeOpen;

    private void Start()
    {
        
        player = GameObject.FindGameObjectWithTag("Player");
        target = GameObject.Find("SkillTreeObject");
        input = player.GetComponent<PlayerInput>();
        playerClass = player.GetComponent<PlayerClass>();

        playerActionMap = input.actions.FindActionMap("Player");
        uiActionMap = input.actions.FindActionMap("UI");


        skillTree.enabled = false;
        hasClass = false;
        openSkillTree = false;

        interactAction = input.actions.FindAction("Interact");

        skillTreeHasBeenOpened = false;

        isSkilltreeOpen = false;


        foreach (Button button in Runner)
        {
            button.interactable = false;
            
        }
        foreach (Button button in Warrior)
        {
            button.interactable = false;
            
        }
        foreach (Button button in Tank)
        {
            button.interactable = false;
            
        }



    }

    private void Update()
    {

        skillPointText.text = skillPoint.ToString();
        if (hasClass)
        {
            ColorBlock colorBlock = Normal.colors;
            colorBlock.disabledColor = Color.gray;
            Reset.interactable = true;
            if (playerClass.hasNormal)
            {
                
                foreach (Button button in Runner)
                {
                    button.interactable = false;
                    button.colors = colorBlock;
                }
                foreach (Button button in Warrior)
                {
                    button.interactable = false;
                    button.colors = colorBlock;
                }
                foreach (Button button in Tank)
                {
                    button.interactable = false;
                    button.colors = colorBlock;
                }
            }
            if (playerClass.hasLifeSteal)
            {
                Normal.colors = colorBlock;
                Normal.interactable = false;
                if (!player.GetComponent<Combat>().hasInvulnerableAbility) Warrior[1].colors = colorBlock;
                foreach (Button button in Runner)
                {
                    button.interactable = false;
                    button.colors = colorBlock;
                }
  
                foreach (Button button in Tank)
                {
                    button.interactable = false;
                    button.colors = colorBlock;
                }
            }
            if (playerClass.hasRunner)
            {
                Normal.colors = colorBlock;
                Normal.interactable = false;
                foreach (Button button in Warrior)
                {
                    button.interactable = false;
                    button.colors = colorBlock;
                }
                foreach (Button button in Tank)
                {
                    button.interactable = false;
                    button.colors = colorBlock;
                }
            }
            if (playerClass.hasTankClass)
            {
                Normal.colors = colorBlock;
                Normal.interactable = false;
                if (!player.GetComponent<Combat>().hasGroundSlam) Tank[1].colors = colorBlock;
                
                foreach (Button button in Runner)
                {
                    button.interactable = false;
                    button.colors = colorBlock;
                }
                foreach (Button button in Warrior)
                {
                    button.interactable = false;
                    button.colors = colorBlock;
                }
            }
        }
        else if (skillPoint >= 1)
        {
            Runner[0].interactable = true;
            Warrior[0].interactable = true;
            Tank[0].interactable = true;
            Normal.interactable = true;
        }
        else
        {
            ColorBlock colorBlock = Normal.colors;
            colorBlock.disabledColor = Color.gray;
            Normal.colors = colorBlock;
            Reset.interactable = true;
            Normal.interactable = true;

            foreach (Button button in Runner)
            {
                button.interactable = false;
                button.colors = colorBlock;
            }
            foreach (Button button in Warrior)
            {
                button.interactable = false;
                button.colors = colorBlock;
            }
            foreach (Button button in Tank)
            {
                button.interactable = false;
                button.colors = colorBlock;
            }
        }



        Collider[] hit = Physics.OverlapSphere(target.transform.position, 5f);

        foreach (var collider in hit)
        {
            if (collider.CompareTag("Player"))
            {
                if (!skillTree.enabled && interactAction.triggered)
                {
                    OpenTree();
                }
            }
        }

        bool backPressed = Keyboard.current.eKey.wasPressedThisFrame ||
                        (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame);

        if (skillTree.enabled && backPressed)
        {
            CloseTree();
        }

        if (skillTree.enabled && Gamepad.current != null)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                GameObject objectToSelect = lastSelectedButton != null ? lastSelectedButton : NormalObject;
                EventSystem.current.SetSelectedGameObject(objectToSelect);
            }
            else if (EventSystem.current.currentSelectedGameObject != lastSelectedButton)
            {
                lastSelectedButton = EventSystem.current.currentSelectedGameObject;
            }
        }

        if (!hasClass)
        {
            Reset.interactable = true;
        }
        else if (hasClass)
        {
            Reset.interactable = true;
        }
    }


    void OpenTree()
    {
        isSkilltreeOpen = true;
        playerActionMap.Disable();
        uiActionMap.Enable();

        skillTree.enabled = true;
        EventSystem.current.SetSelectedGameObject(NormalObject);
        lastSelectedButton = NormalObject;

        player.GetComponent<PlayerMovement>().canRotate = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        skillTreeHasBeenOpened = true;

        if (Gamepad.current != null)
        {
            Cursor.visible = false;
            input.SwitchCurrentActionMap("UI");
        }
    }

    void CloseTree()
    {
        uiActionMap.Disable();
        playerActionMap.Enable();

        isSkilltreeOpen = false;

        skillTree.enabled = false;

        player.GetComponent<PlayerMovement>().canRotate = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ChooseNormalClass();

        if (Gamepad.current != null)
        {
            input.SwitchCurrentActionMap("Player");
        }
    }

    public void ChooseNormalClass()
    {
        if (!hasClass && isSkilltreeOpen)
        {
            hasClass = true;
            playerClass.hasNormal = true;
            ColorBlock colorBlock = Normal.colors;

            colorBlock.disabledColor = Color.white;

            Normal.colors = colorBlock;
            Normal.interactable = false;
            EventSystem.current.SetSelectedGameObject(NormalObject);

        }
        
    }

    public void ChooseRunnerClass()
    {
        if (!hasClass && skillPoint >= 1 && isSkilltreeOpen)
        {
            skillPoint -= 1;
            skillPointsSpend += 1;
            hasClass = true;
            playerClass.hasRunner = true;

            ColorBlock colorBlock = Normal.colors;
            ColorBlock colorBlock2 = Normal.colors;

            colorBlock.disabledColor = Color.yellow;
            colorBlock2.disabledColor = Color.gray;

            Runner[0].colors = colorBlock;
            Runner[2].colors = colorBlock2;
            Runner[0].interactable = false;
            Runner[2].interactable = false;


            if (skillPoint >= 3)
            {
                Runner[1].interactable = true;
                Runner[2].interactable = true;
            }
            else
            {
                Runner[1].interactable = false;
                Runner[2].interactable = false;
            }
            
        }
    }

    public void ChooseJD()
    {
        if (skillPoint >= 3 && isSkilltreeOpen)
        {
            if(playerClass.hasRunner == true)
            {
                ColorBlock colorBlock = Normal.colors;

                colorBlock.disabledColor = Color.yellow;
                Runner[1].colors = colorBlock;
                skillPoint -= 3;
                skillPointsSpend += 3;
                Runner[1].interactable = false;

                player.GetComponent<PlayerMovement>().allowDoubleJump = true;
                if (skillPoint >= 5)
                {
                    Runner[2].interactable = true;
                }
                else
                {
                    Runner[2].interactable = false;
                }
            }
        }
    }

    public void ChooseDA()
    {
        if (skillPoint >= 5 && isSkilltreeOpen)
        {
            if (playerClass.hasRunner == true)
            {
                ColorBlock colorBlock = Normal.colors;

                colorBlock.disabledColor = Color.yellow;
                Runner[2].colors = colorBlock;
                skillPoint -= 5;
                skillPointsSpend += 5;
                Runner[2].interactable = false;


                player.GetComponent<Combat>().attackWhileDash = true;
            }
        }
    }


    public void ResetSkill()
    {
        if(hasClass == true && isSkilltreeOpen)
        {
            hasClass = false;
            if (!playerClass.hasNormal)
            {
                skillPoint += skillPointsSpend;
                skillPointsSpend = 0;
            }
            playerClass.hasNormal = false;
            playerClass.hasLifeSteal = false;
            playerClass.hasTankClass = false;
            playerClass.hasRunner = false;
            player.GetComponent<PlayerMovement>().allowDoubleJump = false;
            player.GetComponent<Combat>().attackWhileDash = false;
            player.GetComponent<Combat>().hasInvulnerableAbility = false;
            player.GetComponent<Combat>().hasGroundSlam = false;
            ColorBlock colorBlock = Normal.colors;

            colorBlock.disabledColor = Color.gray;
            Normal.interactable = true;
            foreach (Button button in Runner)
            {
                button.interactable = false;
                button.colors = colorBlock;
            }
            foreach (Button button in Warrior)
            {
                button.interactable = false;
                button.colors = colorBlock;
            }
            foreach (Button button in Tank)
            {
                button.interactable = false;
                button.colors = colorBlock;
            }
            Warrior[0].interactable = true;
            Tank[0].interactable = true;
            Runner[0].interactable = true;
        }
    }

    public void ChooseWarrior()
    {
        if (!hasClass && skillPoint >= 1 && isSkilltreeOpen)
        {
            skillPoint -= 1;
            skillPointsSpend += 1;
            hasClass = true;
            playerClass.hasLifeSteal = true;

            ColorBlock colorBlock = Normal.colors;

            colorBlock.disabledColor = Color.red;

            Warrior[0].colors = colorBlock;
            
            Warrior[0].interactable = false;
            if (skillPoint >= 5)
            {
                Warrior[1].interactable = true;
            }
            else
            {
                Warrior[1].interactable = false;
            }

        }
    }

    public void ChooseInvul()
    {
        if (skillPoint >= 5 && isSkilltreeOpen)
        {
            if(playerClass.hasLifeSteal == true)
            {
                ColorBlock colorBlock = Normal.colors;

                colorBlock.disabledColor = Color.red;
                skillPoint -= 5;
                skillPointsSpend += 5;
                Warrior[1].colors = colorBlock;
                Warrior[1].interactable = false;
                player.GetComponent<Combat>().hasInvulnerableAbility = true;
            }
        }
    }

    public void ChooseTank()
    {
        if (!hasClass && skillPoint >= 1 && isSkilltreeOpen)
        {
            skillPoint -= 1;
            skillPointsSpend += 1;
            hasClass = true;
            playerClass.hasTankClass = true;

            ColorBlock colorBlock = Normal.colors;

            colorBlock.disabledColor = Color.blue;

            Tank[0].colors = colorBlock;
            
            Tank[0].interactable = false;
            if (skillPoint >= 5)
            {
                Tank[1].interactable = true;
            }
            else
            {
                Tank[1].interactable = false;
            }

        }
    }

    public void ChooseSLAM()
    {
        if (skillPoint >= 5 && isSkilltreeOpen)
        {
            if (playerClass.hasTankClass == true)
            {
                ColorBlock colorBlock = Normal.colors;

                colorBlock.disabledColor = Color.blue;
                Tank[1].colors = colorBlock;
                skillPoint -= 5;
                skillPointsSpend += 5;
                Tank[1].interactable = false;
                player.GetComponent<Combat>().hasGroundSlam = true;
            }
        }
    }
}