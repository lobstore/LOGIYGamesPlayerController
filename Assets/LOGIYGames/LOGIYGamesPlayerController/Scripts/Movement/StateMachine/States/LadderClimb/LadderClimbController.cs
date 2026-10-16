using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class LadderClimbController : MonoBehaviour
    {
        Actor character;
        MovementWrapperBase controller;
        public Ladder Ladder { get; private set; }
        float t;
        float distanceTravelled;
        private void Awake()
        {
            character = GetComponent<Actor>();
            controller = GetComponent<MovementWrapperBase>();
        }
        public void Enter()
        {
            distanceTravelled = 0.01f;
            controller.UseGravity = false;
        }
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
        public Vector3 GetCurrentPosition()
        {
            return Ladder.GetPosition(t);
        }
        public Vector3 GetCurrentDirection()
        {
            return Ladder.GetDirection(t);
        }
        public void Tick()
        {
            distanceTravelled += character.RuntimeMovement.TargetSpeed * character.Input.MovementInput.y * Time.deltaTime;
            t = distanceTravelled / Ladder.Lenght;
            t = Mathf.Clamp01(t);
            Vector3 pos = GetCurrentPosition();

            character.transform.position = pos;
        }
        public void Exit()
        {
            Ladder = null;
            controller.UseGravity = true;
        }
        public bool CanExit()
        {
            return (t <= 0f || t >= 1f) || Ladder == null;
        }
        public bool CanEnter()
        {
            return Ladder != null && character.Input.InteractPressed;
        }
    }
}
