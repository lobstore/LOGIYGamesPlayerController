using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
namespace LOGIYGames
{
    public class Enemy : MonoBehaviour
    {
        EnemyConfig enemyConfig;
        IObjectPool<Enemy> objectPool;

        float currentHealth;
        public float detectionRadius;

        public void Initialize(EnemyConfig enemyConfig, Vector3 spawnPosition, IObjectPool<Enemy> objectPool)
        {
            this.enemyConfig = enemyConfig;
            this.objectPool = objectPool;
            transform.position = spawnPosition;
            currentHealth = enemyConfig.health;
            gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            objectPool?.Release(this);
        }
    }
}