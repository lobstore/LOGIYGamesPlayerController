//using UnityEngine;
//using LOGIYGames;
//public class LedgeClimbingActionContext : MonoBehaviour
//{
//    Character Player;
//    SensorsModule Sensors;
//    PlayerInputsManager HumanoidInput;
//    CountdownTimer climbJumpCooldownTimer;
//    CountdownTimer jumpCooldownTimer;


//    private float climbJumpingCooldown = 0.2f;
//    private float jumpCooldown = 0.2f;

//    public bool IsHanging { get; private set; }
//    public bool IsJumping { get; private set; }

//    private void Awake()
//    {
//        Sensors = GetComponent<SensorsModule>();
//        Player = GetComponent<Character>();
//        HumanoidInput = GetComponent<PlayerInputsManager>();
//        climbJumpCooldownTimer = new CountdownTimer(climbJumpingCooldown);
//        jumpCooldownTimer = new CountdownTimer(jumpCooldown);
//        HumanoidInput.Jumped.AddListener(OnJump);
//        Player.CharacterTimers.Add(climbJumpCooldownTimer);
//        Player.CharacterTimers.Add(jumpCooldownTimer);
//    }
//    private void OnJump()
//    {
//        if (!climbJumpCooldownTimer.IsRunning)
//        {
//            if (HumanoidInput.MovementInput.y > 0)
//            {
//                IsJumping = true;
//                climbJumpCooldownTimer.Start();
//                Player.ExitingWallTimer.Start();
//            }
//        }
//    }
//    private void Update()
//    {
//        CheckForHanging();
//    }

//    private void CheckForHanging()
//    {
//        if (!Player.IsGrounded && Sensors.IsObstcleHeadFront && HumanoidInput.MovementInput.y >= 0)
//        {
//            StartLedgeHanging();

//        }
//        else
//        {
//            if (IsHanging)
//            {
//                StopLedgeHanging();
//            }
//        }
//    }
//    private void StartLedgeHanging()
//    {
//        Player.HorizontalVelocity = Vector3.zero;
//        IsHanging = true;
//        Player.UseGravity = false;
//    }
//    private void StopLedgeHanging()
//    {
//        Player.ExitingWallTimer.Start();
//        IsHanging = false;
//        Player.UseGravity = true;
//    }
//    public void HangMove()
//    {

//        Vector3 wallNormal = Sensors.ForeheadFrontHit.normal;
//        Vector3 magnit = -wallNormal;

//        Vector3 wallAlong = Vector3.Cross(wallNormal, Player.transform.up);
//        Player.RotateToDirection(-wallNormal);
//        var desiredVelocity = (wallAlong * Player.CurrentSpeed * HumanoidInput.MovementInput.x + magnit);

//        Player.HorizontalVelocity = Vector3.Lerp(Player.HorizontalVelocity, desiredVelocity, Player.Acceleration * Time.deltaTime);
//        Player.VerticalVelocity = 0;
//    }
//}