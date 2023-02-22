public interface IInteractable
{
    string GetInteractActionDescription();
    bool CanInteract(Player playerScript);
    void OnInteract(Player playerScript);
}
