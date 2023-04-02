using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StealthProp : MonoBehaviour
{
    private EnemyManager enemyManager;
    private ShadowCaster2D shadowCasterScript;

    private void Awake()
    {
        enemyManager = ManagerAccessor.instance.EnemyManager; 
        shadowCasterScript = GetComponent<ShadowCaster2D>();
    }

    private void Update()
    {
        shadowCasterScript.enabled = enemyManager.IsPlayerSneaking();
    }
}
