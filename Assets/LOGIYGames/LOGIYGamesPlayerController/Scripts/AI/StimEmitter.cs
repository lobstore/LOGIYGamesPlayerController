using LOGIYGames.CharacterCore;
using UnityEngine;

namespace Perception {
    public class StimEmitter : MonoBehaviour {
        [SerializeField] float walkSpeed = 0.5f;
        [SerializeField] float runSpeed = 1f;
        [SerializeField] float sprintSpeed = 1.5f;
        [SerializeField] float walkRadius = 4f;
        [SerializeField] float runRadius = 8f;
        [SerializeField] float sprintRadius = 18f;
        
        CharacterModule player;
        Transform body;

        void Awake() {
            player = GetComponent<CharacterModule>();
            body = transform;
        }

        void Update() {
            if (!player || !body) return;
            var speed = player.VelocityData.Locomotion.magnitude;
            if (speed < walkSpeed) return;
            var loud = speed >= sprintSpeed;
            
            

            PerceptionHub.Emit(
                new Stim(
                    loud ? StimType.AudioLoud : StimType.AudioMovement,
                    body,
                    body.position,
                    ResolveRadius(speed))
                );
        }
        private float ResolveRadius(float speed)
        {
            if (speed<walkSpeed)
            {
                return 0;
            }
            if (speed<runSpeed)
            {
                return walkRadius;
            }
            if (speed < sprintSpeed)
            {
                return runRadius;
            }
            return sprintRadius;
        }
    }
}