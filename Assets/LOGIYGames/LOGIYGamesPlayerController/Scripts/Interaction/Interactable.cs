using UnityEngine;

namespace LOGIYGames
{
    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private InteractionData interactionData;

        public virtual InteractionData GetInteractionData()
        {
            return interactionData;
        }

        public abstract void Interact(GameObject interactor);
    }
}
