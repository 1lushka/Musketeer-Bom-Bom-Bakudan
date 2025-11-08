using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Pool_Enemy))]
public class WaveSpawner : MonoBehaviour
{
    [Header("Спавн")]
    public Transform spawnCenter;
    public float lineLength = 30f;

    [Header("Конфиг")]
    public WaveConfig waveConfig;

    private Pool_Enemy _pool;
    private int _waveIndex = -1;
    private int _alive = 0;

    private void Awake()
    {
        _pool = GetComponent<Pool_Enemy>();
        if (spawnCenter == null) spawnCenter = transform;

        if (waveConfig == null)
        {
            enabled = false;
            return;
        }

        _pool.PrewarmFromWaveConfig(waveConfig);
    }

    private void Start()
    {
        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        while (_waveIndex < waveConfig.waves.Count - 1)
        {
            yield return new WaitUntil(() => _alive == 0);

            if (_waveIndex >= 0)
                yield return new WaitForSeconds(waveConfig.timeBetweenWaves);

            _waveIndex++;
            var wave = waveConfig.waves[_waveIndex];

            var toSpawn = new List<EnemyAbstract>();

            foreach (var rule in wave.enemies)
            {
                if (rule.prefab == null) continue;
                int count = Random.Range(rule.min, rule.max + 1);
                for (int i = 0; i < count; i++)
                    toSpawn.Add(rule.prefab);
            }

            _alive = toSpawn.Count;
            if (_alive == 0) continue;

            
            toSpawn = toSpawn
                .OrderBy(e => wave.enemies.First(r => r.prefab == e).priority)
                .ToList();

            foreach (var prefab in toSpawn)
            {
                Spawn(prefab);
                yield return new WaitForSeconds(waveConfig.timeBetweenSpawns);
            }
        }

        yield return new WaitUntil(() => _alive == 0);
        
    }

    private void Spawn(EnemyAbstract prefab)
    {
        var enemy = _pool.GetEnemy(prefab);
        if (!enemy)
        {
            _alive--;
            return;
        }

        float x = Random.Range(-lineLength * 0.5f, lineLength * 0.5f);
        enemy.transform.position = spawnCenter.position + new Vector3(x, 0, 0);
        enemy.transform.rotation = Quaternion.identity;
        enemy.currentHP = enemy.maxHP;
        enemy.currentSpeed = enemy.speed;

        enemy.OnDeath = null;
        enemy.OnDeath += Die;
    }

    private void Die(EnemyAbstract e)
    {
        _alive--;
        _pool.ReturnEnemy(e);
        e.OnDeath -= Die;
    }
}