using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;

    public float searchRadius = 100f;
    public int groupSize = 3;
    private bool isGroupFormed = false;

    private List<EnemyMovement> nearbyEnemies = new List<EnemyMovement>();

    private static Dictionary<string, List<EnemyMovement>> groups = new Dictionary<string, List<EnemyMovement>>();
    private static Dictionary<string, EnemyMovement> leaders = new Dictionary<string, EnemyMovement>();
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        switch (gameObject.tag)
        {
            case "Weak":
                if (!isGroupFormed)
                {
                    SeekNearbyEnemies();
                    TryFormGroup();
                }
                else
                {
                    if (this == GetGroupLeader())
                    {
                        agent.destination = player.position;
                    }
                    else
                    {
                        agent.destination = GetGroupLeader().transform.position + GetOffsetFromLeader();
                    }
                }
                break;

            default:
                agent.destination = player.position;
                break;
        }
    }
    void SeekNearbyEnemies()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, searchRadius);
        nearbyEnemies.Clear();

        foreach (Collider hit in hits)
        {
            if (hit.gameObject != gameObject && hit.CompareTag(gameObject.tag))
            {
                EnemyMovement other = hit.GetComponent<EnemyMovement>();
                if (other != null && !IsInGroup(other))
                {
                    nearbyEnemies.Add(other);
                }
            }
        }
    }

    void TryFormGroup()
    {
        if (nearbyEnemies.Count >= groupSize - 1 && !IsInGroup(this))
        {
            string type = gameObject.tag;

            if (!groups.ContainsKey(type))
                groups[type] = new List<EnemyMovement>();

            groups[type].Clear();
            groups[type].Add(this);
            groups[type].AddRange(nearbyEnemies.GetRange(0, groupSize - 1));
            leaders[type] = this;

            foreach (EnemyMovement enemy in groups[type])
            {
                enemy.isGroupFormed = true;
            }

            Debug.Log($"Group formed for '{type}' with leader: {leaders[type].name}");
        }
    }

    bool IsInGroup(EnemyMovement enemy)
    {
        string type = enemy.gameObject.tag;
        return groups.ContainsKey(type) && groups[type].Contains(enemy);
    }

    EnemyMovement GetGroupLeader()
    {
        string type = gameObject.tag;
        return leaders.ContainsKey(type) ? leaders[type] : null;
    }

    Vector3 GetOffsetFromLeader()
    {
        string type = gameObject.tag;

        if (!groups.ContainsKey(type)) return Vector3.zero;

        int index = groups[type].IndexOf(this);
        float angle = index * Mathf.PI * 2 / groups[type].Count;
        float radius = 2f;
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
    }
}

