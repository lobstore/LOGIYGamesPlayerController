using UnityEngine;
namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "New Ability Factory", menuName = "Ability/AbilityFactory")]
    public class AbilityFactory : ScriptableObject
    {
        [SerializeField] AbilityData abilityData;

        public virtual Ability Create()
        {
            return new Ability(abilityData);
        }
    }
}
