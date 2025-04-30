using UnityEngine;

public class AttackDetect : MonoBehaviour
{
    GameObject enemy;

    private void Start()
    {
        enemy = GameObject.FindGameObjectWithTag("Enemy");
    }


    void DoAttack()
    {
        if (enemy == null) return;
        enemy.GetComponent<FlockingTest>().DoAttack();
    }
}
