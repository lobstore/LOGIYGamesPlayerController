using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class CharacterVelocityDebug : MonoBehaviour
    {
        CharacterModule characterModule;

        [SerializeField] Color movementTargetDirectionArrowColor;

        private void Awake()
        {
            characterModule = GetComponent<CharacterModule>();
        }

        private void Update()
        {
            var velo = characterModule.TargetDirection * characterModule.BaseSpeed;
            if (velo.magnitude > 0)
            {
                DebugDraw.DrawArrow(transform.position, velo, movementTargetDirectionArrowColor);
            }
        }
    }
}
