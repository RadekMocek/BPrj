using UnityEngine;

public class RigidbodyProp : MonoBehaviour, IDamageable
{
    [Header("Rigidbody settings")]
    [SerializeField] private int weight;

    private Rigidbody2D RB;

    public void ReceiveDamage(Vector2 direction, int amount)
    {
        RB.AddForce(1000 * direction);
    }

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        RB.gravityScale = 0;
        RB.constraints = RigidbodyConstraints2D.FreezeRotation;
        RB.mass = weight;
        RB.drag = weight;
    }
}
