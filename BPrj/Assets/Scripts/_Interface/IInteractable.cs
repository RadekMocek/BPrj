public interface IInteractable
{
    string GetInteractActionDescription();
    void OnInteract(Player playerScript);
}
