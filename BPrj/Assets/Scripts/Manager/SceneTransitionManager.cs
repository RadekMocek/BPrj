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
        // Consistency
        consistencyManagerScript.OnSceneChanged(name);
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

        ///*TODO: For testing purposes, comment me out in release
        if (customSpawnData != null && customSpawnData.Enabled) {
            var data = customSpawnData;
            ChangeScene(data.SceneName, data.PlayerX, data.PlayerY);
        }
        /**/
    }

    ///*TODO: For testing purposes, comment me out in release
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            ChangeScene(GetCurrentSceneName(), playerGO.transform.position.x, playerGO.transform.position.y);
        }
    }
    /**/
}
