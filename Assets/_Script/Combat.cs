using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Combat : MonoBehaviour
{
    public int playerHealth = 100;

    Slider Health;


    private void Start()
    {
        Health = GameObject.Find("Healthbar").GetComponent<Slider>();
        Health.maxValue = playerHealth;
    }

    private void Update()
    {
        Health.value = playerHealth;
        if (Input.GetKeyDown(KeyCode.G))
        {
            playerHealth -= 10;
        }
    }


}
