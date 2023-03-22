using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Persistent GameObjects")]
    [SerializeField] private GameObject playerGO;
    //[SerializeField] private GameObject globalLightGO;
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
        //DontDestroyOnLoad(globalLightGO);
        DontDestroyOnLoad(canvasGO);
        DontDestroyOnLoad(camerasGO);

        playerGO.transform.parent = this.transform;
        //globalLightGO.transform.parent = this.transform;
        canvasGO.transform.SetParent(this.transform, false);
        camerasGO.transform.parent = this.transform;
    }

    //TODO: For testing purposes, delete me in release
    private void Update()
    {
        if (/*Application.isEditor && */Input.GetKeyDown(KeyCode.R)) {
            ChangeScene("Main");
        }
    }

}
