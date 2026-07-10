using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class LadderClimbController : MonoBehaviour
    {
        [SerializeField] Character character;
        public Ladder Ladder { get; private set; }
        public float t {  get; set; }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Ladder ladder))
            {
                Ladder = ladder;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Ladder ladder))
            {
                if (Ladder == ladder)
                    Exit();
            }
        }
        public void Tick()
        {
            Vector3 pos = Ladder.GetPosition(t);
            // выход сверху/снизу
            if (t <= 0f || t >= 1f)
            {
                Exit();
            }

            // прыжок/выход
            if (character.Input.JumpPressed)
            {
                Exit();
            }
            character.transform.position = pos;
        }
        private void Exit()
        {
            Ladder = null;
        }
    }
}
