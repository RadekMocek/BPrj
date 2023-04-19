using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnClickNewGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Outside");
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
}
