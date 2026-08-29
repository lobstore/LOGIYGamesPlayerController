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
        [Header("Transitions")]
        public List<AttackTransition> Transitions = new();
    }
}
