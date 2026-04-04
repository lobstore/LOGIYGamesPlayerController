using UnityEngine;



namespace LOGIYGames.CharacterCore
{
    public class LadderMovementController : MonoBehaviour
    {
        public LadderEndpointTrigger LadderEndpoint { get; private set; }
        private void OnTriggerEnter(Collider other)
        {
            LadderEndpoint = other.GetComponent<LadderEndpointTrigger>();
        }
        private void OnTriggerExit(Collider other)
        {
            LadderEndpoint = null;
        }
    }
}
