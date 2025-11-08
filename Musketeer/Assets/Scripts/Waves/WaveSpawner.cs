using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Pool_Enemy))]
public class WaveSpawner : MonoBehaviour
{
    [Header("Точка спавна")]
    public Transform spawnCenter;

    [Header("Параметры спавна")]
    public float lineLength = 30f;
    

    [Header("Волны")]
    public int enemiesPerWave = 10;
    public float timeBetweenWaves = 0f;
    public float timeBetweenSpawns = 0.5f;

    

    private Pool_Enemy _pool;
    private int _currentWave = 0;
    private int _enemiesAliveThisWave = 0;

    private void Awake()
    {
        _pool = GetComponent<Pool_Enemy>();
        if (spawnCenter == null) spawnCenter = transform;
    }

    private void Start()
    {
        StartCoroutine(WaveController());
    }

    private IEnumerator WaveController()
    {
        while (true)
        {
            yield return new WaitUntil(() => _enemiesAliveThisWave == 0);

            if (_currentWave > 0)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }

            _currentWave++;
            _enemiesAliveThisWave = enemiesPerWave;

            for (int i = 0; i < enemiesPerWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }
    }

    private void SpawnEnemy()
    {
        EnemyAbstract enemy = _pool.GetEnemy();
        if (enemy == null) return;

        Vector3 spawnPos = GetSpawnPosition();
        enemy.transform.position = spawnPos;
        enemy.transform.rotation = Quaternion.identity;

        enemy.currentHP = enemy.maxHP;
        enemy.currentSpeed = enemy.speed;

        
        enemy.OnDeath = null; 
        enemy.OnDeath += OnEnemyDeath;
    }

    private void OnEnemyDeath(EnemyAbstract enemy)
    {
        _enemiesAliveThisWave--;

        _pool.ReturnEnemy(enemy);
   
        enemy.OnDeath -= OnEnemyDeath; //на всякий случай   
    }

    private Vector3 GetSpawnPosition()
    {
            float x = Random.Range(-lineLength * 0.5f, lineLength * 0.5f);
            return spawnCenter.position + new Vector3(x, 0f, 0f);
    }

}