using LOGIYGames;
using Unity.Netcode;
using UnityEngine;
using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;
namespace LOGIYGames
{
    [RequireComponent(typeof(Character))]
    public class FallingActionContext : AerialActionContext
    {
        [SerializeField] float floatingSpeed;
        [Header("Animation Settings")]
        int landingStateHash = Animator.StringToHash("LandingState");
        int isFallingHash = Animator.StringToHash("IsFalling");
        [Header("Timing Settings")]
        [SerializeField] private float landingDuration = 0.1f;
        [SerializeField] private float minFallingTimeToLandingTransition = 0.8f;
        [SerializeField] private float fallingTimeForHardLanding = 1f;
        [SerializeField] private float hardLandingDuration = 1.5f;
        [SerializeField] private bool autoCalculateLandingDuration = false;


        [Header("Component References")]

        // State Management
        private CountdownTimer landingCoolDownTimer;
        private StopwatchTimer fallingTimer;
        public bool IsLanding { get; private set; }
        public float FallingTime => fallingTimer.CurrentTime;

        protected override void Awake()
        {
            base.Awake();
            InitializeTimers();
        }

        private void InitializeTimers()
        {
            landingCoolDownTimer = new CountdownTimer(landingDuration);
            fallingTimer = new StopwatchTimer();

        }

        public void StartFallingTimer() => fallingTimer.Start();

        public void StopFallingTimer()
        {
            fallingTimer.Stop();

            landingCoolDownTimer.Reset(landingDuration);
        }

        public void OnLanding()
        {
            landingCoolDownTimer.Start();
            IsLanding = true;

            SetLandingAnimationState();
        }

        private void SetLandingAnimationState()
        {
            animator.SetInteger(landingStateHash, FallingTime <= fallingTimeForHardLanding
                ? 1 : 2);
        }

        public override void EnterState()
        {
            base.EnterState();
            InternalSpeedMultiplier = floatingSpeed;
            StartFallingTimer();
            animator?.SetBool(isFallingHash, true);
            animator?.SetInteger(landingStateHash, 0);
        }
        public override void ExitState()
        {
            base.ExitState();
            animator?.SetBool(isFallingHash, false);
            animator?.SetInteger(landingStateHash, 0);
            StopFallingTimer();

        }

        private void Update()
        {
            //if (!IsOwner) return;

            if (landingCoolDownTimer.IsFinished)
            {
                IsLanding = false;

            }
        }
    }
}