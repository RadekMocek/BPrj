using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour, IRightClickable
{
    public void OnRightClick(Player playerScript)
    {
        var playerGO = playerScript.gameObject;
        var distanceFromPlayer = Vector2.Distance(playerGO.transform.position, this.transform.position);
        Debug.Log($"Successful right click! {distanceFromPlayer}");

        if (distanceFromPlayer <= 1) {
            // Equip
        }
    }
}
