using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public float enemyHealth = 100f;


    public SkinnedMeshRenderer enemyRenderer;
    MaterialPropertyBlock propBlock;


    private FlockingTest flockingTest;

    private void Start()
    {
        enemyRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        
        flockingTest = GetComponent<FlockingTest>();

        propBlock = new MaterialPropertyBlock();
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
        StartCoroutine(FlashRed());
        
    }

    private IEnumerator FlashRed()
    {

        enemyRenderer.GetPropertyBlock(propBlock);


        propBlock.SetColor("_Color", Color.red);

        enemyRenderer.SetPropertyBlock(propBlock);

        yield return new WaitForSeconds(0.2f);


       
   
        enemyRenderer.SetPropertyBlock(propBlock);
    }
}