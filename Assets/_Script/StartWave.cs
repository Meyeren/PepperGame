using UnityEngine;
using UnityEngine.InputSystem;

public class StartWave : MonoBehaviour
{
    public GameObject target;
    public GameObject button;

    public GameObject Waves;

    private bool whenStartWave;

    private void Start()
    {
        whenStartWave = false;
    }

    void Update()
    {
        if(Waves.GetComponent<EnemyWaves>().enemiesAlive == 0)
        {
            whenStartWave = true;
            button.SetActive(true);

        }
        else
        {
            whenStartWave = false;
            button.SetActive(false);
        }

        Collider[] hit = Physics.OverlapSphere(target.transform.position, 5f);

        bool interactPressed = Keyboard.current.eKey.wasPressedThisFrame ||
       (Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame);

        foreach (var collider in hit)
        {
            if (collider.CompareTag("Player"))
            {
                if (interactPressed && whenStartWave == true)
                {
                    Waves.GetComponent<EnemyWaves>().StartNextWave();
                }
            }
        }
    }
}
