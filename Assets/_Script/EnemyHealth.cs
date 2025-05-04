using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float enemyHealth = 100f;

    public float maxEnemyHealth = 100f;

    private FlockingTest flockingTest;

    Renderer mat;
    Color originalMatColor;


    private void Start()
    {
        flockingTest = GetComponent<FlockingTest>();
        mat = GetComponentInChildren<SkinnedMeshRenderer>();
        originalMatColor = mat.material.color;
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
        
        mat.material.color = Color.red;
        
        Invoke("StopColor", 0.1f);
    }

    void StopColor()
    {
        mat.material.color = originalMatColor;
        
    }
}