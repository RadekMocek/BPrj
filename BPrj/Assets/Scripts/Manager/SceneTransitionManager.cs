using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Persistent GameObjects")]
    [SerializeField] private GameObject playerGO;
    [SerializeField] private GameObject canvasGO;
    [SerializeField] private GameObject camerasGO;

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    private void Awake()
    {
        DontDestroyOnLoad(playerGO);
        DontDestroyOnLoad(canvasGO);
        DontDestroyOnLoad(camerasGO);
    }
}
