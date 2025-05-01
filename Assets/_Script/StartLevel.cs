using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartLevel : MonoBehaviour
{
    PlayerClass playerClass;
    GameObject target;
    bool skillTree;
    public GameObject skillTreeOb;

    private void Update()
    {
        skillTree = skillTreeOb.GetComponent<skillTreeManager>().skillTreeHasBeenOpened;

        target = GameObject.Find("StartDoor");

        Collider[] hit = Physics.OverlapSphere(target.transform.position, 5f);

        bool interactPressed = Keyboard.current.eKey.wasPressedThisFrame ||
       (Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame);

        foreach (var collider in hit)
        {
            if (collider.CompareTag("Player"))
            {
                if (interactPressed && skillTree)
                {
                    collider.transform.position = new Vector3(12f, -2f, 0f);
                }
            }
        }
    }
}
