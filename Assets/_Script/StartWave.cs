using UnityEngine;
using UnityEngine.InputSystem;

public class StartWave : MonoBehaviour
{
    public GameObject target;
    public GameObject button;

    public GameObject Waves;
    public GameObject shop;

    public float range = 5f;

    private bool whenStartWave;
    private bool canOpenShop;

    Renderer mat;
    Color orgin;

    private void Start()
    {
        whenStartWave = false;
        mat = button.GetComponent<Renderer>();
        orgin = mat.material.color;
        
    }

    void Update()
    {
        canOpenShop = shop.GetComponent<ShopManager>().canOpenShop;
        if (Waves.GetComponent<EnemyWaves>().enemiesAlive == 0)
        {
            whenStartWave = true;
            button.SetActive(true);
            

        }
        else
        {
            whenStartWave = false;
            button.SetActive(false);
        }

       

        Collider[] hit = Physics.OverlapSphere(target.transform.position, range);

        bool interactPressed = Keyboard.current.eKey.wasPressedThisFrame ||
       (Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame);

        foreach (var collider in hit)
        {
            if (collider.CompareTag("Player"))
            {
                if(canOpenShop == true)
                {
                    mat.material.color = Color.blue;
                }
                else
                {
                    mat.material.color = Color.red;
                }
                if (interactPressed && whenStartWave == true && canOpenShop == true)
                {
                    shop.GetComponent<ShopManager>().OpenShop();
                }
                else if (interactPressed && whenStartWave == true && canOpenShop == false)
                {
                    Waves.GetComponent<EnemyWaves>().StartNextWave();
                    canOpenShop = true;
                }
                else { return; }
            }
            else
            {
                mat.material.color = Color.grey;
            }
        }
    }
}
