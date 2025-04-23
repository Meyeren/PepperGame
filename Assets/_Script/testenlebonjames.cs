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
    private static Dictionary<string, FormationType> groupFormations = new Dictionary<string, FormationType>();

    private static Dictionary<string, float> groupAttackAngles = new Dictionary<string, float>();

    private static Dictionary<EnemyMovement, List<EnemyMovement>> bossGroups = new Dictionary<EnemyMovement, List<EnemyMovement>>();
    private static Dictionary<EnemyMovement, FormationType> bossFormations = new Dictionary<EnemyMovement, FormationType>();

    public enum FormationType { Circle, Arrow }
    public FormationType chosenFormation = FormationType.Circle;

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
                        agent.destination = GetGroupAttackPosition();
                    }
                    else
                    {
                        if (IsInBossGroup(this))
                        {
                            agent.destination = GetBossLeader(this).transform.position + GetBossOffset();
                        }
                        else
                        {
                            agent.destination = GetGroupLeader().transform.position + GetOffsetFromLeader();
                        }
                    }
                }
                break;

            case "Strong":
                if (!isGroupFormed)
                {
                    SeekNearbyEnemies();
                    TryFormGroup();
                }
                else
                {
                    if (this == GetGroupLeader())
                    {
                        agent.destination = GetGroupAttackPosition();
                    }
                    else
                    {
                        if (IsInBossGroup(this))
                        {
                            agent.destination = GetBossLeader(this).transform.position + GetBossOffset();
                        }
                        else
                        {
                            agent.destination = GetGroupLeader().transform.position + GetOffsetFromLeader();
                        }

                    }
                }
                break;

            case "Boss":
                if (!isGroupFormed)
                {
                    TryFormBossGroup();
                }

                if (IsInBossGroup(this) && GetBossLeader(this) != this)
                {
                    agent.destination = GetBossLeader(this).transform.position + GetBossOffset();
                }
                else
                {
                    agent.destination = player.position;
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

    void TryFormBossGroup()
    {
        if (!IsInBossGroup(this))
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, searchRadius);
            List<EnemyMovement> followers = new List<EnemyMovement>();

            foreach (Collider hit in hits)
            {
                if (hit.gameObject != gameObject && (hit.CompareTag("Weak") || hit.CompareTag("Strong")))
                {
                    EnemyMovement enemy = hit.GetComponent<EnemyMovement>();
                    if (enemy != null && !IsInBossGroup(enemy))
                    {
                        followers.Add(enemy);
                    }
                }
            }

            int countToAdd = Mathf.Min(groupSize, followers.Count);
            bossGroups[this] = new List<EnemyMovement> { this };
            bossGroups[this].AddRange(followers.GetRange(0, countToAdd));
            bossFormations[this] = chosenFormation;

            foreach (var follower in bossGroups[this])
            {
                follower.isGroupFormed = true;
                follower.chosenFormation = chosenFormation;
            }

            Debug.Log($"Boss group formed with leader: {name}, size: {bossGroups[this].Count}");
        }
    }

    void TryFormGroup()
    {
        if (nearbyEnemies.Count > 0 && !IsInGroup(this))
        {
            string type = gameObject.tag;

            if (!groups.ContainsKey(type))
                groups[type] = new List<EnemyMovement>();

            groups[type].Clear();
            groups[type].Add(this);

            int countToAdd = Mathf.Min(groupSize - 1, nearbyEnemies.Count);
            groups[type].AddRange(nearbyEnemies.GetRange(0, countToAdd));
            leaders[type] = this;

            groupFormations[type] = chosenFormation;

            foreach (EnemyMovement enemy in groups[type])
            {
                enemy.isGroupFormed = true;
            }

            if (!groupAttackAngles.ContainsKey(type))
            {
                float angleStep = 180f;
                int index = groupAttackAngles.Count % 4;
                float angle = index * angleStep;
                groupAttackAngles[type] = angle;
            }

            Debug.Log($"Group formed for '{type}' with leader: {leaders[type].name} (size: {groups[type].Count})");
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

        if (!groupFormations.ContainsKey(type) || groupFormations[type] == FormationType.Circle)
            return GetCircleFormationOffset();

        return GetArrowFormationOffset();
    }

    Vector3 GetGroupAttackPosition()
    {
        string type = gameObject.tag;
        if (!groupAttackAngles.ContainsKey(type)) return player.position;

        float angleDeg = groupAttackAngles[type];
        float angleRad = angleDeg * Mathf.Deg2Rad;
        float distanceFromPlayer = 6f;

        Vector3 offset = new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad)) * distanceFromPlayer;
        return player.position + offset;
    }

    Vector3 GetCircleFormationOffset()
    {
        string type = gameObject.tag;

        int index = groups[type].IndexOf(this);
        float angle = index * Mathf.PI * 2 / groups[type].Count;
        float radius = 2f;
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
    }

    Vector3 GetArrowFormationOffset()
    {
        string type = gameObject.tag;

        int index = groups[type].IndexOf(this);
        if (index == 0) return Vector3.zero; 

        float spacing = 2f;
        int side = (index % 2 == 0) ? 1 : -1; 
        int row = (index + 1) / 2;

        float x = side * spacing * row;
        float z = -spacing * row;
        return new Vector3(x, 0, z);
    }

    bool IsInBossGroup(EnemyMovement enemy)
    {
        foreach (var group in bossGroups.Values)
        {
            if (group.Contains(enemy))
                return true;
        }
        return false;
    }

    EnemyMovement GetBossLeader(EnemyMovement enemy)
    {
        foreach (var kvp in bossGroups)
        {
            if (kvp.Value.Contains(enemy))
                return kvp.Key;
        }
        return null;
    }

    Vector3 GetBossOffset()
    {
        EnemyMovement leader = GetBossLeader(this);
        if (leader == null || !bossGroups.ContainsKey(leader)) return Vector3.zero;

        int index = bossGroups[leader].IndexOf(this);
        if (index == 0) return Vector3.zero;

        if (!bossFormations.ContainsKey(leader) || bossFormations[leader] == FormationType.Circle)
            return GetCircleOffsetFromBoss(index, bossGroups[leader].Count);

        return GetArrowOffsetFromBoss(index);
    }

    Vector3 GetCircleOffsetFromBoss(int index, int count)
    {
        float angle = index * Mathf.PI * 2 / count;
        float radius = 3f;
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
    }

    Vector3 GetArrowOffsetFromBoss(int index)
    {
        float spacing = 2f;
        int side = (index % 2 == 0) ? 1 : -1;
        int row = (index + 1) / 2;
        float x = side * spacing * row;
        float z = -spacing * row;
        return new Vector3(x, 0, z);
    }

}

