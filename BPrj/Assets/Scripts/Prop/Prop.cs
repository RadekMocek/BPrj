using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour, IDamageable
{
    private Rigidbody2D RB;

    public void ReceiveDamage(Vector2 direction)
    {
        RB.AddForce(1000 * direction);
    }

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }
}
