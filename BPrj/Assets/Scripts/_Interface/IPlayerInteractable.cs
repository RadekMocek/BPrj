public interface IPlayerInteractable
{
    string GetInteractActionDescription();
    bool CanInteract(Player playerScript);
    void OnInteract(Player playerScript);
}
