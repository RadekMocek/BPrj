public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player player) : base(player)
    {
    }

    private float movementSpeed = 4.5f;

    public override void Update()
    {
        base.Update();

        // Animation logic
        if (movementInput.y > 0) {
            anim.CrossFade("Player_Walk_Up", 0);
            player.LastMovementDirection = 0;
        }
        else if (movementInput.y < 0) {
            anim.CrossFade("Player_Walk_Down", 0);
            player.LastMovementDirection = 2;
        }
        else if (movementInput.x < 0) {
            anim.CrossFade("Player_Walk_Left", 0);
            player.LastMovementDirection = 3;
        }
        else if (movementInput.x > 0) {
            anim.CrossFade("Player_Walk_Right", 0);
            player.LastMovementDirection = 1;
        }

        // Update weapon position according to movement direction
        UpdateWeaponPosition();

        // Movement logic
        player.SetVelocity(movementSpeed * movementInput);

        // ChangeState logic
        if (movementInput.magnitude == 0) {
            player.ChangeState(player.IdleState);
        }
        else if (sneakInputPressedThisFrame) {
            player.ChangeState(player.SneakMoveState);
        }
    }
}
