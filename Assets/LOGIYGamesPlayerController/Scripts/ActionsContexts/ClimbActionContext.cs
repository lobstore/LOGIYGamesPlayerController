//using LOGIYGames;
//using UnityEngine;
//[RequireComponent(typeof(CharacterModule))]
//[RequireComponent(typeof(PlayerInputsManager))]
//[RequireComponent(typeof(SensorsModule))]
//public class ClimbActionContext : MonoBehaviour, IActionContext
//{
//    SensorsModule Sensors;
//    CharacterModule player;
//    PlayerInputsManager HumanoidInput;
//    CinemachineCameraSwitcher CameraManager;
//    private CountdownTimer climbingTimer;
//    private CountdownTimer climbJumpCooldownTimer;
//    [SerializeField] private float climbingWallTime = 1f;
//    [SerializeField] private float climbJumpingCooldown = 0.3f;
//    [SerializeField] private int jumpForce = 4;
//    [SerializeField] private float wallJumpForce = 4;
//    [SerializeField] private float maxWallLookAngle = 30f;
//    [field: SerializeField] public float Acceleration { get; private set; } = 100f;
//    [field: SerializeField] public float ClimbDeceleration { get; private set; } = 1f;
//    [field: SerializeField] public float ClimbJumpDeceleration { get; private set; } = 1f;
//    [field: SerializeField] public float Deceleration { get; private set; } = 25f;
//    public float InternalSpeedMultiplier { get; private set; }
//    public bool IsSprinting { get; private set; }
//    public bool IsClimbing { get; private set; }
//    public bool IsJumping { get; set; }
//    [SerializeField] float SprintClimbSpeedMultiplier;
//    [SerializeField] float ClimbSpeedMultiplier;
//    [field: SerializeField] public MotionType MotionType { get; private set; }

//    void Awake()
//    {
//        player = GetComponent<CharacterModule>();
//        HumanoidInput = GetComponent<PlayerInputsManager>();
//        CameraManager = GetComponent<CinemachineCameraSwitcher>();
//        Sensors = GetComponent<SensorsModule>();
//        climbingTimer = new CountdownTimer(climbingWallTime);
//        climbJumpCooldownTimer = new CountdownTimer(climbJumpingCooldown);
//        player.PlayerTimers.Add(climbingTimer);
//        player.PlayerTimers.Add(climbJumpCooldownTimer);
//        HumanoidInput.Jumped.AddListener(OnJump);
//    }
//    private void OnJump()
//    {
//        if (!climbJumpCooldownTimer.IsRunning)
//        {
//            if (Sensors.IsObstacleLegsFront && !player.IsGrounded)
//            {
//                IsJumping = true;
//                player.ExitingWallTimer.Start();
//                climbJumpCooldownTimer.Start();
//            }
//        }
//    }
//    public void Climb()
//    {

//        player.RotateToDirection(-Sensors.LegsFrontHit.normal, 0);
//        player.VerticalVelocity = player.CurrentSpeed;
//    }
//    public void ClimbJump()
//    {
//        StopWallClimbing();
//        player.VerticalVelocity = Mathf.Sqrt(jumpForce * -2 * Physics.gravity.y);
//        player.HorizontalVelocity = Sensors.LegsFrontHit.normal * wallJumpForce;
//        player.RotateToDirection(Sensors.LegsFrontHit.normal, 0);
//    }
//    private void StartWallClimbing()
//    {
//        IsClimbing = true;
//        climbingTimer.Resume();
//    }
//    private void StopWallClimbing()
//    {
//        IsClimbing = false;
//        climbingTimer.Stop();
//        player.ExitingWallTimer.Start();

//    }
//    private void CheckForWallClimbing()
//    {
//        if (player.IsGrounded)
//        {
//            climbingTimer.Reset();
//        }
//        if (CanClimbWall())
//        {
//            if (!IsClimbing && climbingTimer.Progress >= 0 && !player.ExitingWallTimer.IsRunning)
//            {
//                StartWallClimbing();
//            }
//            if (climbingTimer.IsFinished)
//            {
//                StopWallClimbing();
//            }

//        }
//        else
//        {
//            if (IsClimbing)
//            {
//                StopWallClimbing();

//            }

//        }
//    }

//    private bool CanClimbWall()
//    {
//        return Sensors.IsObstacleLegsFront
//                    && HumanoidInput.MovementInput.y > 0
//                    && Vector3.Angle(player.transform.forward, -Sensors.LegsFrontHit.normal) < maxWallLookAngle
//                    && Vector3.Angle(player.transform.forward, Camera.main.transform.forward) < 30;
//    }

//    public void SpeedControl()
//    {
//        if (HumanoidInput.MovementInput.magnitude > 0.5f)
//        {


//            if (HumanoidInput.IsShifting)
//            {
//                InternalSpeedMultiplier = SprintClimbSpeedMultiplier;
//            }
//            else
//            {
//                InternalSpeedMultiplier = ClimbSpeedMultiplier;
//            }
//        }
//        else
//        {
//            InternalSpeedMultiplier = 0f;
//        }
//    }
//    private void Update()
//    {
//        CheckForWallClimbing();
//        if (climbJumpCooldownTimer.IsFinished)
//        {
//            IsJumping = false;
//        }
//    }

//    public void EnterState()
//    {
//        player.Acceleration = Acceleration;
//        player.Deceleration = Deceleration;
//    }

//    public void ExitState()
//    {

//    }

//    public void OnUpdate()
//    {
//        throw new System.NotImplementedException();
//    }

//    public void OnFixedUpdate()
//    {
//        throw new System.NotImplementedException();
//    }
//}