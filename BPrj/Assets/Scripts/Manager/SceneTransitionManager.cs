using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private CustomSpawnSO customSpawnData;

    [Header("Persistent GameObjects")]
    [SerializeField] private GameObject playerGO;
    [SerializeField] private GameObject canvasGO;
    [SerializeField] private GameObject camerasGO;

    private ConsistencyManager consistencyManagerScript;
    private Player playerScript;
    private Vector2 tempVector;

    public string GetCurrentSceneName() => SceneManager.GetActiveScene().name;

    public void ChangeScene(string name, float playerX, float playerY)
    {
        SceneManager.LoadScene(name);
        StartCoroutine(ChangeSceneAfterLogic(name, playerX, playerY));
    }

    private IEnumerator ChangeSceneAfterLogic(string name, float playerX, float playerY)
    {
        // Wait until the scene is fully loaded
        while (GetCurrentSceneName() != name) yield return null;
        // Teleport player to desired coordinates
        tempVector.Set(playerX, playerY);
        playerGO.transform.position = tempVector;
        playerScript.OnSceneChanged();
        // Hide tutorial popup
        ManagerAccessor.instance.HUD.HideTutorialPopup();
        // Consistency
        consistencyManagerScript.OnSceneChanged(name);
    }

    public void Respawn()
    {
        ChangeScene("RespawnMiddleman", 0, 0);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Destroy(ManagerAccessor.instance.gameObject);
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

        consistencyManagerScript = ManagerAccessor.instance.ConsistencyManager;
        playerScript = playerGO.GetComponent<Player>();

        // Debug
        if (Application.isEditor && customSpawnData != null && customSpawnData.Enabled) {
            var data = customSpawnData;
            ChangeScene(data.SceneName, (customSpawnData.CustomCors) ? data.PlayerX : 0, (customSpawnData.CustomCors) ? data.PlayerY : 0);
        }
    }
}
