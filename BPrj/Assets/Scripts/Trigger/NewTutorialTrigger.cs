public class NewTutorialTrigger : Trigger
{
    protected override void TriggerLogic()
    {
        ManagerAccessor.instance.HUD.NewTutorial();
    }
}
