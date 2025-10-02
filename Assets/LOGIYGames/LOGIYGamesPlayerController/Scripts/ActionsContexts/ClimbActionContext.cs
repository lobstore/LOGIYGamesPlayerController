//using LOGIYGames;
//using UnityEngine;
//[RequireComponent(typeof(Character))]
//[RequireComponent(typeof(PlayerInputsManager))]
//[RequireComponent(typeof(Sensors))]
//public class ClimbActionContext : MonoBehaviour, IActionContext
//{
//    Sensors Sensors;
//    Character Character;
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
//        Character = GetComponent<Character>();
//        HumanoidInput = GetComponent<PlayerInputsManager>();
//        CameraManager = GetComponent<CinemachineCameraSwitcher>();
//        Sensors = GetComponent<Sensors>();
//        climbingTimer = new CountdownTimer(climbingWallTime);
//        climbJumpCooldownTimer = new CountdownTimer(climbJumpingCooldown);
//        Character.CharacterTimers.Add(climbingTimer);
//        Character.CharacterTimers.Add(climbJumpCooldownTimer);
//        HumanoidInput.Jumped.AddListener(OnJump);
//    }
//    private void OnJump()
//    {
//        if (!climbJumpCooldownTimer.IsRunning)
//        {
//            if (Sensors.IsObstacleLegsFront && !Character.IsGrounded)
//            {
//                IsJumping = true;
//                Character.ExitingWallTimer.Start();
//                climbJumpCooldownTimer.Start();
//            }
//        }
//    }
//    public void Climb()
//    {

//        Character.RotateToDirection(-Sensors.LegsFrontHit.normal, 0);
//        Character.VerticalVelocity = Character.CurrentSpeed;
//    }
//    public void ClimbJump()
//    {
//        StopWallClimbing();
//        Character.VerticalVelocity = Mathf.Sqrt(jumpForce * -2 * Physics.gravity.y);
//        Character.HorizontalVelocity = Sensors.LegsFrontHit.normal * wallJumpForce;
//        Character.RotateToDirection(Sensors.LegsFrontHit.normal, 0);
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
//        Character.ExitingWallTimer.Start();

//    }
//    private void CheckForWallClimbing()
//    {
//        if (Character.IsGrounded)
//        {
//            climbingTimer.Reset();
//        }
//        if (CanClimbWall())
//        {
//            if (!IsClimbing && climbingTimer.Progress >= 0 && !Character.ExitingWallTimer.IsRunning)
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
//                    && Vector3.Angle(Character.transform.forward, -Sensors.LegsFrontHit.normal) < maxWallLookAngle
//                    && Vector3.Angle(Character.transform.forward, Camera.main.transform.forward) < 30;
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
//        Character.Acceleration = Acceleration;
//        Character.Deceleration = Deceleration;
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