using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Persistent GameObjects")]
    [SerializeField] private GameObject playerGO;
    [SerializeField] private GameObject canvasGO;
    [SerializeField] private GameObject camerasGO;

    public string GetCurrentSceneName() => SceneManager.GetActiveScene().name;

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    private void Awake()
    {
        DontDestroyOnLoad(playerGO);
        DontDestroyOnLoad(canvasGO);
        DontDestroyOnLoad(camerasGO);

        playerGO.transform.parent = this.transform;
        canvasGO.transform.SetParent(this.transform, false);
        camerasGO.transform.parent = this.transform;

        canvasGO.SetActive(true);
    }

    //TODO: For testing purposes, delete me in release
    private void Update()
    {
        if (/*Application.isEditor && */Input.GetKeyDown(KeyCode.R)) {
            ChangeScene("Main");
        }
    }

}
