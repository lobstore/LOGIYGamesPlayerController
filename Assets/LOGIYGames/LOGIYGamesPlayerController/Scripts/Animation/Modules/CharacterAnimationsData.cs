using UnityEngine;

namespace LOGIYGames.Animation
{
    [CreateAssetMenu(fileName = "CharacterAnimationsData", menuName = "CharacterAnimationsData")]
    public class CharacterAnimationsData : ScriptableObject
    {
        public int Dash_Left { get; } = Animator.StringToHash(nameof(Dash_Left));
        public int Dash_Right { get; } = Animator.StringToHash(nameof(Dash_Right));
        public int Dash_Forward { get; } = Animator.StringToHash(nameof(Dash_Forward));
        public int Dash_Backward { get; } = Animator.StringToHash(nameof(Dash_Backward));

        public int Jump_Grounded_Left { get; } = Animator.StringToHash(nameof(Jump_Grounded_Left));
        public int Jump_Grounded_Right { get; } = Animator.StringToHash(nameof(Jump_Grounded_Right));
        public int Jump_Grounded_Forward { get; } = Animator.StringToHash(nameof(Jump_Grounded_Forward));
        public int Jump_Grounded_Backward { get; } = Animator.StringToHash(nameof(Jump_Grounded_Backward));
        public int Jump_Grounded_Up { get; } = Animator.StringToHash(nameof(Jump_Grounded_Up));
        public int Jump_Braced_Backward { get; } = Animator.StringToHash(nameof(Jump_Braced_Backward));

        public int Run_Stop_Left { get; } = Animator.StringToHash(nameof(Run_Stop_Left));
        public int Run_Stop_Right { get; } = Animator.StringToHash(nameof(Run_Stop_Right));
        public int Run_Stop_Forward { get; } = Animator.StringToHash(nameof(Run_Stop_Forward));
        public int Run_Stop_Backward { get; } = Animator.StringToHash(nameof(Run_Stop_Backward));
        public int Run_BackTurn_Right { get; } = Animator.StringToHash(nameof(Run_BackTurn_Right));
        public int Run_BackTurn_Left { get; } = Animator.StringToHash(nameof(Run_BackTurn_Left));

        public int Sprint_Stop_Forward{ get; } = Animator.StringToHash(nameof(Sprint_Stop_Forward));
        public int Sprint_BackTurn_Left { get; } = Animator.StringToHash(nameof(Sprint_BackTurn_Left));

        public int Walk_Stop_Left { get; } = Animator.StringToHash(nameof(Walk_Stop_Left));
        public int Walk_Stop_Right { get; } = Animator.StringToHash(nameof(Walk_Stop_Right));
        public int Walk_Stop_Forward { get; } = Animator.StringToHash(nameof(Walk_Stop_Forward));
        public int Walk_Stop_Backward { get; } = Animator.StringToHash(nameof(Walk_Stop_Backward));
        public int Walk_BackTurn_Right { get; } = Animator.StringToHash(nameof(Walk_BackTurn_Right));
        public int Walk_BackTurn_Left { get; } = Animator.StringToHash(nameof(Walk_BackTurn_Left));

        public int Idle_BackTurn_Right { get; } = Animator.StringToHash(nameof(Idle_BackTurn_Right));
        public int Idle_BackTurn_Left { get; } = Animator.StringToHash(nameof(Idle_BackTurn_Left));

        public int Roll_Forward { get; } = Animator.StringToHash(nameof(Roll_Forward));
        public int Roll_Backward { get; } = Animator.StringToHash(nameof(Roll_Backward));
        public int Roll_Right { get; } = Animator.StringToHash(nameof(Roll_Right));
        public int Roll_Left { get; } = Animator.StringToHash(nameof(Roll_Left));
        public int Landing_Light_Idle { get; } = Animator.StringToHash(nameof(Landing_Light_Idle));
        public int Landing_Hard_Idle { get; } = Animator.StringToHash(nameof(Landing_Hard_Idle));


    }
}
