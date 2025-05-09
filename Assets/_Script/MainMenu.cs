using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public bool isButtonClickable;
    public Button newGameButton;
    public Button SettingsButton;
    public Button CloseSettingsButton;
    public Button CreditsButton;
    public Button ExitButton;
    public GameObject SettingsPanel;
    public EventSystem eventSystem;

    void Start()
    {
        SettingsPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
    }

    public void newGame()
    {
        if (!isButtonClickable)
        {
            return;
        }
        else
        {
            SceneManager.LoadScene("Finalize");
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
            Test.SaveToCSV();

            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    public void openSettings()
    {
        if (!isButtonClickable)
        {
            return;
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(CloseSettingsButton.gameObject);
            newGameButton.enabled = false;
            SettingsButton.enabled = false;
            CreditsButton.enabled = false;
            ExitButton.enabled = false;
            SettingsPanel.SetActive(true); 
        }
    }

    public void closeSettings()
    {
        if (!isButtonClickable)
        {
            return;
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(SettingsButton.gameObject);
            newGameButton.enabled = true;
            SettingsButton.enabled = true;
            CreditsButton.enabled = true;
            ExitButton.enabled = true;
            SettingsPanel.SetActive(false);
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
