using UnityEngine;

public class Weapon : MonoBehaviour, IRightClickable
{
    private BoxCollider2D BC;
    
    //private bool equipped;
    //private Player playerScript;

    public void OnRightClick(Player playerScript)
    {
        //var playerGO = playerScript.gameObject;
        var distanceFromPlayer = Vector2.Distance(playerScript.transform.position, this.transform.position);

        if (distanceFromPlayer <= 1.5f) {
            playerScript.EquipWeapon(this.gameObject);
            BC.enabled = false;
            //this.playerScript = playerScript;
            //equipped = true;
        }
    }

    private void Awake()
    {
        BC = GetComponent<BoxCollider2D>();
    }

    /*
    private void Start()
    {
        equipped = false;
    }

    private void Update()
    {
        if (equipped) {
            // Update position and rotation according to cursor position
            var playerToCursorDirection = playerScript.GetPlayerToCursorDirection();
            this.transform.localPosition = playerToCursorDirection + (Vector2)playerScript.Center.localPosition;
            int angle = (int)(Mathf.Rad2Deg * Mathf.Atan2(playerToCursorDirection.y, playerToCursorDirection.x));
            this.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            
        }
    }
    */
}
