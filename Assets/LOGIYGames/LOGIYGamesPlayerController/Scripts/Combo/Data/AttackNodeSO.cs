using LOGIYGames.Shared.Enums;
using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    [CreateAssetMenu(menuName = "Combat/Attack Node")]
    public class AttackNodeSO : ScriptableObject
    {
        [Header("Animation")]
        public AnimationData Animation;

        [Header("Timing")]
        [Min(0f)]
        public float TotalDuration = 0.8f;

        [Header("Windows")]
        public TimeWindow ComboInputWindow;
        public TimeWindow DodgeCancelWindow;

        [Header("Transitions")]
        public List<AttackTransition> Transitions = new();
    }
}
