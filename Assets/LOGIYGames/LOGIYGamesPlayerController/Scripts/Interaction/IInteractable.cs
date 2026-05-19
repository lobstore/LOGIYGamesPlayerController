using UnityEngine;

namespace LOGIYGames
{
    public interface IInteractable
    {
        void Interact(GameObject interactor);

        InteractionData GetInteractionData();
    }
}
