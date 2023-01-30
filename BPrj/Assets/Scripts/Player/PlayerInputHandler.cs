using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput PI { get; set; }

    // Information about pressed keys is being read from these properties
    public int MovementX { get; private set; } // -1 = left; 1 = right; 0 = both/none
    public int MovementY { get; private set; } // -1 = down; 1 = up; 0 = both/none

    private void Awake()
    {
        PI = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        // MovementX
        bool isLeftPressed = PI.actions["Left"].IsPressed();
        bool isRightPressed = PI.actions["Right"].IsPressed();
        MovementX = -Convert.ToInt32(isLeftPressed) + Convert.ToInt32(isRightPressed);

        // MovementY
        bool isUpPressed = PI.actions["Up"].IsPressed();
        bool isDownPressed = PI.actions["Down"].IsPressed();
        MovementY = Convert.ToInt32(isUpPressed) - Convert.ToInt32(isDownPressed);
    }
}
