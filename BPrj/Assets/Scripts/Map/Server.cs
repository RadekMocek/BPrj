using UnityEngine;

public class Server : MonoBehaviour, IObservable, IDamageable, IObservableHealth
{
    // == Observe ===============================
    public virtual string GetName() => "Server";

    // == Health, damage ========================
    private int maxHealth;
    private int health;

    public (int, int) GetHealthInfo() => (health, maxHealth);

    public void ReceiveDamage(Vector2 direction, int amount)
    {
        CameraShake.Instance.ShakeCamera();
        health -= amount;
        if (health <= 0) { //TODO: vypnout zde player sprite (?)
            ManagerAccessor.instance.SceneManager.ChangeScene("Outside_End", -3, 1.8f);
        }
    }

    // == MonoBehaviour functions ===============
    private void Start()
    {
        maxHealth = 100;
        health = maxHealth;
    }
}
