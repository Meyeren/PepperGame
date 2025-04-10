using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{

    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        if (Input.GetButton("Jump"))
        {
            RaycastHit hit;

            /*if (Physics.Raycast(Component.CompareTag("Player"), out hit, 100))
            {
                agent.destination = hit.point;
            }*/
        }
    }
}

