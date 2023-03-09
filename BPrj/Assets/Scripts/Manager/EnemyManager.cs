using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Player reference")]
    [SerializeField] private Transform player;

    public Vector2 GetPlayerPosition() => player.position;
}
