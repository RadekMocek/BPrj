public class PlayerAttackLightState : PlayerAttackSuperState
{
    public PlayerAttackLightState(Player player) : base(player)
    {
        backSwingDuration = 0.15f;
        backSwingSpeed = 85;

        swingCircularSectorAngle = 80;
        swingSpeed = 1000;
        swingDistanceFromCore = 0.8f;
        damageDistanceFromCore = 1.0f;
        damageRadius = 0.6f;
        slipSpeed = 1.0f;
    }
}
