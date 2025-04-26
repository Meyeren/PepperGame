using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLevel : MonoBehaviour
{
    [SerializeField] string arenaName;
    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(arenaName);
    }
}
