using UnityEngine;
namespace LOGIYGames
{
    [CreateAssetMenu(menuName = "Game/EnemyConfig")]
    public class EnemyConfig : ScriptableObject
    {
        public GameObject prefab;
        public float health;

    }
}