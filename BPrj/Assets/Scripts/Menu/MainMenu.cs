using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnClickNewGame()
    {
        SceneManager.LoadScene("Outside");
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
}
