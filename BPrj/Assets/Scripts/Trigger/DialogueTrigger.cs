using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;

    private void Update()
    {
        if (Physics2D.OverlapCircle(this.transform.position, 1, playerLayer)) {
            Debug.Log("trigger");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, 1.0f);
    }
}
