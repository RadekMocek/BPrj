public interface IPlayerInteractable
{
    string GetInteractActionDescription(Player playerScript);
    bool CanInteract(Player playerScript);
    void OnInteract(Player playerScript);
}
