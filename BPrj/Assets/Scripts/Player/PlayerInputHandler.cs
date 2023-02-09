using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    // Component references
    private PlayerInput PI { get; set; }

    // Available actions
    private InputAction leftAction;
    private InputAction rightAction;
    private InputAction upAction;
    private InputAction downAction;
    public InputAction SneakAction { get; private set; }

    // Information about pressed keys can be read from these properties
    public int MovementX { get; private set; } // -1 = left; 1 = right; 0 = both/none
    public int MovementY { get; private set; } // -1 = down; 1 = up; 0 = both/none

    // MonoBehaviour functions
    private void Awake()
    {
        PI = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        // Initialize actions
        leftAction = PI.actions["Left"];
        rightAction = PI.actions["Right"];
        upAction = PI.actions["Up"];
        downAction = PI.actions["Down"];
        SneakAction = PI.actions["Sneak"];
    }

    private void Update()
    {
        // Update properties
        MovementX = -Convert.ToInt32(leftAction.IsPressed()) + Convert.ToInt32(rightAction.IsPressed());
        MovementY = Convert.ToInt32(upAction.IsPressed()) - Convert.ToInt32(downAction.IsPressed());
    }
}
