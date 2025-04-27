using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float enemyHealth = 100f;

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

    public void TakeDamage(float Damage)
    {
        enemyHealth -= Damage;
    }
}