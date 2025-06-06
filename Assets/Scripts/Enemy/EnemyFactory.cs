using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
namespace LOGIYGames
{
    public class EnemyFactory : IEntityFactory<Enemy, EnemyConfig>
    {
        readonly Transform parent;
        readonly Dictionary<EnemyConfig, IObjectPool<Enemy>> pools = new();

        public EnemyFactory(Transform parent = null)
        {
            this.parent = parent;
        }

        public Enemy Create(EnemyConfig config, Vector3 position)
        {
            var pool = pools.TryGetValue(config, out var existingPool)
                ? existingPool
                : pools[config] = new ObjectPool<Enemy>(
                    () => Object.Instantiate(config.prefab, parent).GetComponent<Enemy>(),
                    enemy => enemy.gameObject.SetActive(true),
                    enemy => enemy.gameObject.SetActive(false),
                    enemy => Object.Destroy(enemy.gameObject),
                    true, 50, 50);
            Enemy enemy = pool.Get();
            enemy.Initialize(config, position, pool);
            return enemy;
        }


    }
}