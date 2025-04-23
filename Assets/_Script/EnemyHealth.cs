using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int enemyHealth = 100;

    private FlockingTest flockingTest;

    private void Start()
    {
        flockingTest = GetComponent<FlockingTest>();
    }

    private void Update()
    {

        if (enemyHealth <= 0)
        {
            flockingTest.StateMachine.ChangeState(new Death(flockingTest));
        }
    }

    public void TakeDamage(int Damage)
    {
        enemyHealth -= Damage;
    }
}