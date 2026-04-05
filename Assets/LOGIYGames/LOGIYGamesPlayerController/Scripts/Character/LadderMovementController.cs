using UnityEngine;



namespace LOGIYGames.CharacterCore
{
    public class LadderMovementController : MonoBehaviour
    {
        [SerializeField] Character character;
        public Ladder Ladder { get; private set; }
        public bool LadderInFrontLegs { get; private set; }
        private void Update()
        {
            RaycastHit raycastHit;
            if (!Physics.Raycast(transform.position + Vector3.up * 0.1f, transform.forward, out raycastHit, 0.5f))
            {
                Ladder = null;
                LadderInFrontLegs = false;
                return;
            }

            Ladder = raycastHit.collider.GetComponent<Ladder>();
            if (Ladder!=null)
            {
                LadderInFrontLegs = true;
            }
            else
            {
                LadderInFrontLegs = false;
            }



        }
    }
}
