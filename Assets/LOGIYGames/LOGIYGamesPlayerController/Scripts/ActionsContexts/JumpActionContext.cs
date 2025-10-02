using UnityEngine;
using UnityEngine.InputSystem;
using LOGIYGames.Timers;
namespace LOGIYGames
{
    public class JumpActionContext : AerialActionContext
    {

        [Header("Jump Settings")]
        [SerializeField] private float jumpVerticalImpulse = 1.5f;
        [SerializeField] private float jumpPlanarImpulse = 1f;
        [SerializeField] private float jumpCooldown = 0.2f;
        [SerializeField] private int maxJumpCount = 2;
        CharacterGravityModule characterGravityModule;

        // State Variables
        private CountdownTimer jumpCooldownTimer;
        private int currentJumpCount;
        public bool IsJumping { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            InitializeJumpSystem();
        }

        private void InitializeJumpSystem()
        {
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            TimersManager.RegisterTimer(jumpCooldownTimer);
            currentJumpCount = maxJumpCount;
            characterGravityModule = GetComponent<CharacterGravityModule>();
        }

        private void OnEnable()
        {
            jumpCooldownTimer.Reset(jumpCooldown);
            jumpCooldownTimer.OnTimerStart += StartJump;
            jumpCooldownTimer.OnTimerStop += StopJump;

        }



        private void OnDisable()
        {
            jumpCooldownTimer.OnTimerStart -= StartJump;
            jumpCooldownTimer.OnTimerStop -= StopJump;

        }
        private void Update()
        {
            if (currentJumpCount > 0 && Character.JumpPressed && !jumpCooldownTimer.IsRunning)
            {
                jumpCooldownTimer.Start();
            }
        }

        private void StartJump()
        {
            IsJumping = true;
        }
        private void StopJump()
        {
            IsJumping = false;
        }

        private void ExecuteJump()
        {
            switch (MotionType)
            {
                case MotionType.CharacterController:
                    {
                        characterGravityModule.VerticalVelocity = Mathf.Sqrt(jumpVerticalImpulse * -2f * Physics.gravity.y);
                        if (MovementInput.magnitude > 0)
                        {
                            Vector3 movement = new Vector3(MovementInput.x, 0, MovementInput.y);

                            Vector3 cam = Camera.main.transform.forward;
                            
                            Character.HorizontalVelocity += Quaternion.LookRotation(new Vector3(cam.x, 0, cam.z)) * movement * Character.TotalSpeedMultiplier * jumpPlanarImpulse;
                        }
                    }
                    break;
                default:
                    break;
            }
            animator?.CrossFade("JumpUpward", 0.05f);
            currentJumpCount--;
            Character.JumpPressed = false;

        }

        public override void EnterState()
        {
            base.EnterState();
            ExecuteJump();
        }

        public void ResetJump(int newJumpCount = -1)
        {
            IsJumping = false;
            currentJumpCount = newJumpCount >= 0 ? newJumpCount : maxJumpCount;
        }
    }
}