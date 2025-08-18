//using UnityEngine;
//using LOGIYGames;
//[RequireComponent(typeof(CharacterModule))]
//[RequireComponent(typeof(PlayerInputsManager))]
//[RequireComponent(typeof(SensorsModule))]
//public class WallRunActionContext : MonoBehaviour, IActionContext
//{
//    SensorsModule Sensors;
//    PlayerInputsManager HumanoidLocomotionInput;
//    CinemachineCameraSwitcher CameraManager;
//    CharacterModule Player;
//    private float turnSmoothTime;
//    CountdownTimer wallJumpCooldownTimer;
//    CountdownTimer runningWallTimer;
//    [SerializeField] float wallJumpCooldown = 0.2f;
//    [SerializeField] float runningWallTime = 1f;
//    [SerializeField] private float wallRunGravityMultiplier = 0f;
//    [SerializeField] private float jumpForce = 4f;
//    [SerializeField] private float wallJumpForce = 4f;
//    [field: SerializeField] public MotionType MotionType { get; private set; }
//    [field: SerializeField] public float Acceleration { get; set; } = 100f;
//    [field: SerializeField] public float Deceleration { get; set; } = 10f;

//    [field: SerializeField] public float InternalSpeedMultiplier { get; set; } = 1f;
//    [SerializeField] private bool useWallCliping = true;
//    public bool IsWallRunning { get; set; }
//    public bool IsJumping { get; set; }

//    private void OnJump()
//    {
//        if (!wallJumpCooldownTimer.IsRunning)
//        {
//            if ((Sensors.IsObstacleLegsLeft || Sensors.IsObstacleLegsRight) && !Player.IsGrounded && HumanoidLocomotionInput.MovementInput.y > 0)
//            {
//                IsJumping = true;
//                wallJumpCooldownTimer.Start();
//                Player.ExitingWallTimer.Start();
//            }
//        }
//    }
//    public void WallJump()
//    {
//        StopWallRunning();
//        Vector3 wallNormal;
//        if (Sensors.IsObstacleLegsRight)
//        {
//            wallNormal = Sensors.LegsRightHit.normal;
//        }
//        else
//        {
//            wallNormal = Sensors.LegsLeftHit.normal;
//        }

//        Player.VerticalVelocity = Mathf.Sqrt(jumpForce * -2 * Physics.gravity.y);
//        Player.HorizontalVelocity = wallNormal * wallJumpForce + Player.transform.forward * Player.CurrentSpeed;

//        Player.RotateToDirection(Player.HorizontalVelocity);
//    }
//    private void CheckForWallRunning()
//    {
//        if (Player.IsGrounded)
//        {
//            runningWallTimer.Reset();
//        }
//        if (CanWallRun())
//        {
//            if (!IsWallRunning && runningWallTimer.Progress >= 0 && !Player.ExitingWallTimer.IsRunning)
//            {
//                StartWallRunning();
//            }
//            if (runningWallTimer.IsFinished)
//            {
//                StopWallRunning();
//            }
//        }
//        else
//        {
//            if (IsWallRunning)
//            {
//                StopWallRunning();

//            }
//        }
//    }

//    private bool CanWallRun()
//    {
//        return (Sensors.IsObstacleLegsLeft || Sensors.IsObstacleLegsRight)
//                    && !Player.IsGrounded
//                    && HumanoidLocomotionInput.MovementInput.y > 0
//                    && !Sensors.IsObstacleLegsFront
//                    && Vector3.Angle(Player.transform.forward, Camera.main.transform.forward) < 60;
//    }

//    private void StopWallRunning()
//    {
//        runningWallTimer.Stop();
//        Player.ExitingWallTimer.Start();
//        IsWallRunning = false;
//    }

//    private void StartWallRunning()
//    {
//        runningWallTimer.Resume();
//        IsWallRunning = true;
//    }
//    public void WallRunMove()
//    {

//        Vector3 wallNormal = Sensors.IsObstacleLegsRight ? Sensors.LegsRightHit.normal : Sensors.LegsLeftHit.normal;
//        Vector3 magnit;

//        Vector3 wallAlong = Vector3.Cross(wallNormal, Player.transform.up);
//        if ((Player.transform.forward - wallAlong).magnitude > (Player.transform.forward + wallAlong).magnitude)
//        {
//            wallAlong = -wallAlong;
//        }

//        if (IsWallRunning && useWallCliping)
//        {
//            magnit = -wallNormal;
//        }
//        else
//        {
//            magnit = Vector3.zero;
//        }

//        Player.RotateToDirection(wallAlong, turnSmoothTime);
//        if (Player.VerticalVelocity <= 0)
//        {
//            Player.VerticalVelocity = Physics.gravity.y * wallRunGravityMultiplier;

//        }
//        var desiredVelocity = (wallAlong * Player.CurrentSpeed + magnit);
//        Player.HorizontalVelocity = Vector3.Lerp(Player.HorizontalVelocity, desiredVelocity, Acceleration * Time.deltaTime);
//    }

//    void Awake()
//    {
//        HumanoidLocomotionInput = GetComponent<PlayerInputsManager>();
//        Player = GetComponent<CharacterModule>();
//        Sensors = GetComponent<SensorsModule>();
//        wallJumpCooldownTimer = new CountdownTimer(wallJumpCooldown);
//        runningWallTimer = new CountdownTimer(runningWallTime);
//        Player.PlayerTimers.Add(wallJumpCooldownTimer);
//        Player.PlayerTimers.Add(runningWallTimer);
//        HumanoidLocomotionInput.Jumped.AddListener(OnJump);
//    }

//    void Update()
//    {
//        CheckForWallRunning();
//        if (wallJumpCooldownTimer.IsFinished)
//        {
//            IsJumping = false;
//        }
//    }

//    public void EnterState()
//    {

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