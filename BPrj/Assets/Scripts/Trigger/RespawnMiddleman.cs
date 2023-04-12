using UnityEngine;

public class RespawnMiddleman : MonoBehaviour
{
    private void Awake()
    {
        ManagerAccessor.instance.SceneManager.ChangeScene("Floor1", 0.5f, -4);
    }
}
