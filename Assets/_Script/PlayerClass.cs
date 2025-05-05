using UnityEngine;
using System.Collections;

public class PlayerClass : MonoBehaviour
{
    Combat combat;
    PlayerMovement move;


    [Header("Bool")]
    public bool hasTankClass = false;
    public bool hasLifeSteal = false;
    public bool hasRunner = false;
    public bool hasNormal = false;

    [Header("NormalClass")]
    public float normalSpeed = 8f;
    public float normalDamage = 50f;
    public float normalHealth = 100f;



    [Header("TankClass")]
    [SerializeField]int tankHealth = 200;
    [SerializeField] float tankSpeed = 5f;

    [Header("WarriorClass")]
    public int warriorHealth = 20;
    public int warriorDamage;

    [Header("RunnerClass")]
    [SerializeField] int runnerHealth = 50;
    [SerializeField] float runnerSpeed = 10f;
    [SerializeField] float runnerDamage = 20;

    private void Start()
    {
        combat = GetComponent<Combat>();
        move = GetComponent<PlayerMovement>();

        
    }

    public void ForceClass()
    {
        if (hasNormal)
        {
            combat.playerHealth = normalHealth;
            combat.MaxPlayerHealth = normalHealth;
            move.Speed = normalSpeed;
            combat.basicDamage = normalDamage;
        }

        if (hasTankClass)
        {
            combat.playerHealth = tankHealth;
            combat.MaxPlayerHealth = tankHealth;
            move.Speed = tankSpeed;
            combat.basicDamage = normalDamage;
        }

        if (hasLifeSteal)
        {
            combat.playerHealth = warriorHealth;
            combat.MaxPlayerHealth = warriorHealth;
            move.Speed = normalSpeed;
            combat.basicDamage = warriorDamage;
        }

        if (hasRunner)
        {
            combat.playerHealth = runnerHealth;
            combat.MaxPlayerHealth = runnerHealth;
            move.Speed = runnerSpeed;
            combat.basicDamage = runnerDamage;
        }
    }
}
