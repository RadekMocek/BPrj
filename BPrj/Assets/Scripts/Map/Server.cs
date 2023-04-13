using UnityEngine;

public class Server : MonoBehaviour, IObservable, IDamageable, IObservableHealth
{
    [SerializeField] private GameObject deadServerParticlePrefab;
    [SerializeField] private Enemy[] floor3enemyScripts;

    // == Observe ===============================
    public virtual string GetName() => "AI Server";

    // == Health, damage ========================
    private int maxHealth;
    private int health;

    public (int, int) GetHealthInfo() => (health, maxHealth);

    public void ReceiveDamage(Vector2 direction, int amount)
    {
        CameraShake.Instance.ShakeCamera();
        health -= amount;
        if (health <= 0) {
            Instantiate(deadServerParticlePrefab, this.transform.position, Quaternion.identity);
            this.GetComponent<SpriteRenderer>().enabled = false;

            foreach (Enemy enemyScript in floor3enemyScripts) {
                enemyScript.ReceiveDamage(Vector2.zero, int.MaxValue);
            }

            Invoke(nameof(Cutscene), 1.5f);
        }
    }

    private void Cutscene()
    {
        ManagerAccessor.instance.SceneManager.ChangeScene("Outside_End", -3, 1.8f);
    }

    // == MonoBehaviour functions ===============
    private void Start()
    {
        maxHealth = 100;
        health = maxHealth;
    }
}
