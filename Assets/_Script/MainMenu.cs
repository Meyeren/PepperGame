using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Drawing;

public class MainMenu : MonoBehaviour
{
    public bool isButtonClickable;
    public void newGame()
    {
        if (!isButtonClickable)
        {
            return;
        }
        else
        {
            SceneManager.LoadScene(5);
        }
    }

    public void closeGame() 
    {
        if (!isButtonClickable)
        {
            return;
        }
        else
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    public void noClick()
    {
        if (!isButtonClickable)
        {
            return;
        }
    }
}
