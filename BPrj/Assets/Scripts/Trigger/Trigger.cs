using UnityEngine;

public class Trigger : MonoBehaviour
{
    [Header("Trigger")]
    [SerializeField] private float radius = 1.0f;
    [SerializeField] private LayerMask playerLayer;

    protected bool triggered;

    protected virtual void TriggerLogic()
    {

    }

    protected virtual void Start()
    {
        triggered = false;
    }

    private void FixedUpdate()
    {
        if (!triggered && Physics2D.OverlapCircle(this.transform.position, radius, playerLayer)) {
            triggered = true;
            ManagerAccessor.instance.ConsistencyManager.SetRecord(this.transform.name, false);
            TriggerLogic();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, radius);
    }
}
