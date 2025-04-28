using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using TMPro.Examples;

public class skillTreeManager : MonoBehaviour
{
    PlayerInput input;


    GameObject player;
    GameObject target;

    PlayerClass playerClass;

    [Header("Player")]
    public bool hasClass;

    public int skillPoint;

    [Header("UI")]
    public Canvas skillTree;
    public TextMeshProUGUI skillTreeText;
    public TextMeshProUGUI skillPointText;



    [Header("Arrays")]
    public Button[] Runner;
    public Button[] Warrior;
    public Button[] Tank;
    public Button Normal;
    public Button Reset;



    private void Start()
    {
        
        player = GameObject.FindGameObjectWithTag("Player");
        target = GameObject.Find("SkillTreeObject");
        input = player.GetComponent<PlayerInput>();
        playerClass = player.GetComponent<PlayerClass>();

        skillTree.enabled = false;
        hasClass = false;

        skillPoint = PlayerPrefs.GetInt("SkillPoint");
        


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
            Reset.interactable = false;
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
                OpenTree();
            }
            else
            {
                if (Gamepad.current == null)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                

                player.GetComponent<PlayerMovement>().canRotate = true;
                skillTree.enabled = false;
            }
        }

        if (!hasClass)
        {
            Reset.interactable = false;
        }
        else if (hasClass)
        {
            Reset.interactable = true;
        }
    }


    void OpenTree()
    {
        if (Gamepad.current == null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Gamepad.current != null)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        player.GetComponent<PlayerMovement>().canRotate = false;
        skillTree.enabled = true;
    }

    public void ChooseNormalClass()
    {
        if (!hasClass)
        {
            hasClass = true;
            playerClass.hasNormal = true;
            ColorBlock colorBlock = Normal.colors;

            colorBlock.disabledColor = Color.cyan;

            Normal.colors = colorBlock;
            Normal.interactable = false;

        }
        
    }

    public void ChooseRunnerClass()
    {
        if (!hasClass && skillPoint >= 1)
        {
            skillPoint -= 1;
            hasClass = true;
            playerClass.hasRunner = true;

            ColorBlock colorBlock = Normal.colors;
            ColorBlock colorBlock2 = Normal.colors;

            colorBlock.disabledColor = Color.cyan;
            colorBlock2.disabledColor = Color.gray;

            Runner[0].colors = colorBlock;
            Runner[2].colors = colorBlock2;
            Runner[0].interactable = false;
            Runner[2].interactable = false;
            if (skillPoint >= 3)
            {
                Runner[1].interactable = true;
            }
            else
            {
                Runner[1].interactable = false;
            }
            
        }
    }

    public void ChooseJD()
    {
        if (skillPoint >= 3)
        {
            ColorBlock colorBlock = Normal.colors;

            colorBlock.disabledColor = Color.cyan;
            Runner[1].colors = colorBlock;
            skillPoint -= 3;
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

    public void ChooseDA()
    {
        if (skillPoint >= 5)
        {
            ColorBlock colorBlock = Normal.colors;

            colorBlock.disabledColor = Color.cyan;
            Runner[2].colors = colorBlock;
            skillPoint -= 5;
            Runner[2].interactable = false;
            player.GetComponent<Combat>().attackWhileDash = true;
        }
    }


    public void ResetSkill()
    {
        hasClass = false;
        if (!playerClass.hasNormal)
        {
            skillPoint += 1;
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

    public void ChooseWarrior()
    {
        if (!hasClass && skillPoint >= 1)
        {
            skillPoint -= 1;
            hasClass = true;
            playerClass.hasLifeSteal = true;

            ColorBlock colorBlock = Normal.colors;

            colorBlock.disabledColor = Color.cyan;

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
        if (skillPoint >= 5)
        {
            ColorBlock colorBlock = Normal.colors;

            colorBlock.disabledColor = Color.cyan;
            skillPoint -= 5;
            Warrior[1].colors = colorBlock;
            Warrior[1].interactable = false;
            player.GetComponent<Combat>().hasInvulnerableAbility = true;
        }
    }

    public void ChooseTank()
    {
        if (!hasClass && skillPoint >= 1)
        {
            skillPoint -= 1;
            hasClass = true;
            playerClass.hasTankClass = true;

            ColorBlock colorBlock = Normal.colors;

            colorBlock.disabledColor = Color.cyan;

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
        if (skillPoint >= 5)
        {
            ColorBlock colorBlock = Normal.colors;

            colorBlock.disabledColor = Color.cyan;
            Tank[1].colors = colorBlock;
            skillPoint -= 5;
            Tank[1].interactable = false;
            player.GetComponent<Combat>().hasGroundSlam = true;
        }
    }
}