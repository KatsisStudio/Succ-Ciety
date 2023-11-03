using LewdieJam.Player;

namespace LewdieJam.Map
{
    public interface IInteractible
    {
        public bool CanInteract(PlayerController pc);
        public void Interact();
    }
}
