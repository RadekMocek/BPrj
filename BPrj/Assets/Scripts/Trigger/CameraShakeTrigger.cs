public class CameraShakeTrigger : Trigger
{
    protected override void TriggerLogic() => CameraShake.Instance.ShakeCamera();
}
