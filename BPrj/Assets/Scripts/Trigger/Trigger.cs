using UnityEngine;

public class Trigger : MonoBehaviour
{
    [Header("Trigger")]
    [SerializeField] private LayerMask playerLayer;

    private bool triggered;

    protected virtual void TriggerLogic()
    {

    }

    private void Start()
    {
        triggered = false;
    }

    private void FixedUpdate()
    {
        if (!triggered && Physics2D.OverlapCircle(this.transform.position, 1, playerLayer)) {
            triggered = true;
            TriggerLogic();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, 1.0f);
    }
}
