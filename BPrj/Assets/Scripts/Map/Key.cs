using System.Collections;
using UnityEngine;

public class Key : MonoBehaviour, IPlayerInteractable
{
    // == IPlayerInteractable ===================
    public string GetInteractActionDescription(Player playerScript) => "Sebrat klíè";

    public bool CanInteract(Player playerScript)
    {
        return (Vector2.Distance(playerScript.transform.position, this.transform.position) <= 1.5f);
    }

    public void OnInteract(Player playerScript)
    {
        playerScript.EquippedKeys.Add(color);
        ManagerAccessor.instance.ConsistencyManager.SetRecord(this.transform.name, false);
        Destroy(this.gameObject);
    }

    // == Lock, Key =============================
    [SerializeField] private LockColor color;

    // == Floating effect =======================
    private Vector2 initialPosition;
    private Vector2 currentPosition;
    private int floatingOffset;
    private int floatingAddition;

    private IEnumerator FloatingEffect()
    {
        while (true) {
            if (floatingOffset == 0 || floatingOffset == 10) floatingAddition *= -1;
            floatingOffset += floatingAddition;
            currentPosition.Set(initialPosition.x, initialPosition.y + floatingOffset * 0.02f);
            this.transform.position = currentPosition;
            yield return new WaitForSeconds(0.075f);
        }
    }

    // == MonoBehaviour functions ===============
    private void Start()
    {
        initialPosition = this.transform.position;
        floatingOffset = 0;
        floatingAddition = -1;
        StartCoroutine(FloatingEffect());
    }
}
