using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int wave = 0;

    public void StartNextWave()
    {
        wave++;
        Debug.Log("Wave " + wave + " starter!");
        // Spawn fjender, reset timer osv.
    }
}
