using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartLevel : MonoBehaviour
{
    PlayerClass playerClass;
    GameObject target;
    bool skillTree;
    public GameObject skillTreeOb;

    GameObject player;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        target = GameObject.Find("StartDoor");

        playerClass = player.GetComponent<PlayerClass>();

        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<SphereCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && skillTree)
        {
            collider.transform.position = new Vector3(12f, -2f, 0f);
            playerClass.ForceClass();
        }
    }


    private void Update()
    {
        skillTree = skillTreeOb.GetComponent<skillTreeManager>().hasClass;

        if (skillTree)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<SphereCollider>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<SphereCollider>().enabled = false;
        }
    }
}
