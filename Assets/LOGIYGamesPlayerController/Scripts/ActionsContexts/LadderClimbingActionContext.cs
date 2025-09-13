//using LOGIYGames;
//using Unity.Netcode;
//using UnityEditor.Animations;
//using UnityEngine;
//public class LadderClimbingActionContext : NetworkBehaviour, IActionContext
//{
//    SensorsModule Sensors;
//    [SerializeField] private Animator animator;
//    private PlayerInputsManager MovementInput;
//    AnimatorState climbingAnimation;
//    int ladderClimbDownStartedTriggerHash = Animator.StringToHash("LadderClimbingDownStarted");
//    int ladderClimbUpStartedTriggerHash = Animator.StringToHash("LadderClimbingUpStarted");
//    int isLadderClimbingUpHash = Animator.StringToHash("IsLadderClimbingUp");
//    int isLadderClimbingDownHash = Animator.StringToHash("IsLadderClimbingDown");
//    int ladderClimbingUpEndedTriggerHash = Animator.StringToHash("LadderClimbingUpEnded");
//    int ladderClimbingDownEndedTriggerHash = Animator.StringToHash("LadderClimbingDownEnded");
//    Character Character;
//    CharacterController cc;
//    Vector3 ladderPosition;
//    Vector3 ladderRotation;
//    bool IsClimbing;
//    public bool IsLadderClimbRequisted { get; private set; }
//    public MotionType MotionType => throw new System.NotImplementedException();
//    private void Awake()
//    {
//        Character = GetComponent<Character>();
//        cc = GetComponent<CharacterController>();
//    }
//    private void OnEnable()
//    {
//        MovementInput = PlayerInputsManager.Instance;
//        IsLadderClimbRequisted = false;
//        IsClimbing = false;
//    }
//    public void OnUpdate()
//    {
//        if (!IsOwner) return;

//    }
//    public void OnFixedUpdate()
//    {
//        if (!IsOwner) return;
//        if (MovementInput.MovementInput.y == 0)
//        {
//            animator.SetBool(isLadderClimbingUpHash, false);
//            animator.SetBool(isLadderClimbingDownHash, false);

//        }
//        if (MovementInput.MovementInput.y > 0)
//        {
//            animator.SetBool(isLadderClimbingUpHash, true);
//            animator.SetBool(isLadderClimbingDownHash, false);
//        }
//        if (MovementInput.MovementInput.y < 0)
//        {
//            animator.SetBool(isLadderClimbingUpHash, false);
//            animator.SetBool(isLadderClimbingDownHash, true);
//        }
//    }
//    public void EnterState()
//    {
//        Character.UseGravity = false;
//        Character.ResetMotion();
//        CharacterToLadder();
//    }
//    private void CharacterToLadder()
//    {
//        var targetPosition = new Vector3(ladderPosition.x, Character.transform.position.y, ladderPosition.z);
//        var direction = (targetPosition - Character.transform.position).normalized;
//        float distanceToLadder = Vector3.Distance(Character.transform.position, targetPosition);
//        //Character.transform.position = direction * distanceToLadder;
//        cc.Move(direction * distanceToLadder);
//        Character.RotateToDirection(ladderRotation);
//    }
//    public void ExitState()
//    {

//        Character.UseGravity = true;
//    }
//    private void OnAnimationExit()
//    {
//        IsLadderClimbRequisted = false;
//    }
//    private void OnTriggerEnter(Collider other)
//    {
//        if (!IsClimbing)
//        {

//            if (other.CompareTag("LadderDown"))
//            {
//                ladderPosition = other.transform.position;
//                ladderRotation = other.transform.forward;
//                IsLadderClimbRequisted = true;
//                IsClimbing = true;
//                animator.SetTrigger(ladderClimbDownStartedTriggerHash);
//            }
//            else if (other.CompareTag("LadderUp"))
//            {
//                ladderPosition = other.transform.position;
//                ladderRotation = other.transform.forward;
//                IsLadderClimbRequisted = true;
//                IsClimbing = true;
//                animator.SetTrigger(ladderClimbUpStartedTriggerHash);
//            }
//        }
//        else
//        {
//            if (other.CompareTag("LadderDown"))
//            {
//                ladderPosition = other.transform.position;
//                ladderRotation = other.transform.forward;
//                IsClimbing = false;
//                animator.SetTrigger(ladderClimbingDownEndedTriggerHash);
//            }
//            else if (other.CompareTag("LadderUp"))
//            {
//                ladderPosition = other.transform.position;
//                ladderRotation = other.transform.forward;
//                IsClimbing = false;
//                animator.SetTrigger(ladderClimbingUpEndedTriggerHash);
//            }
//        }
//    }
//}