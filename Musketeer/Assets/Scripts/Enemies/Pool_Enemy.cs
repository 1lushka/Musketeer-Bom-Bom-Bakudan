using System.Collections.Generic;
using UnityEngine;


public class Pool_Enemy : MonoBehaviour
{
    [Header("Настройки пула")]
    public EnemyAbstract enemyPrefab;       
    public int poolSize = 20;                 

    private Queue<EnemyAbstract> _pool = new Queue<EnemyAbstract>(); //решил даже реализоать через очердь

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            EnemyAbstract enemy = Instantiate(enemyPrefab, transform);
            enemy.gameObject.SetActive(false);
            _pool.Enqueue(enemy);
        }
    }

    public EnemyAbstract GetEnemy()
    {
        if (_pool.Count == 0)
        {
            EnemyAbstract enemy = Instantiate(enemyPrefab, transform); //расширяем пул если вышли за границы
            enemy.gameObject.SetActive(false);
            return enemy;
        }

        EnemyAbstract pooledEnemy = _pool.Dequeue();
        pooledEnemy.gameObject.SetActive(true);
        return pooledEnemy;
    }

    public void ReturnEnemy(EnemyAbstract enemy)
    {
        enemy.gameObject.SetActive(false);
        enemy.transform.SetParent(transform);
        _pool.Enqueue(enemy);
    }

}