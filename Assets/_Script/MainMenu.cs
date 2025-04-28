using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Drawing;

public class MainMenu : MonoBehaviour
{
    public void newGame()
    {
        SceneManager.LoadScene(0);
    }

    public void closeGame() 
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
