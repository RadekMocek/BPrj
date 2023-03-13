using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Player transform")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerCore;

    public Vector2 GetPlayerPosition() => player.position;
    public Vector2 GetPlayerCorePosition() => playerCore.position;
}
