using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private HUDManager HUD;

    private bool paused;

    private void Awake()
    {
        HUD = ManagerAccessor.instance.HUD;
    }

    private void Start()
    {
        paused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!paused) {
                paused = true;
                HUD.ShowPause(true);
                Time.timeScale = 0;
            }
            else {
                paused = false;
                HUD.ShowPause(false);
                if (!HUD.IsTutorialShown) Time.timeScale = 1;
            }
        }
    }

    public void OnClickContinue()
    {
        paused = false;
        HUD.ShowPause(false);
        Time.timeScale = 1;
    }

    public void OnClickMenu()
    {
        ManagerAccessor.instance.SceneManager.MainMenu();
    }
}
