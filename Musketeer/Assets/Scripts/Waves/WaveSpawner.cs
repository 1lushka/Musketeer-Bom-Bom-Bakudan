using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Pool_Enemy))]
public class WaveSpawner : MonoBehaviour
{
    [Header("Спавн")]
    [SerializeField] private Transform spawnCenter;
    [SerializeField] private float lineLength = 30f;

    [Header("Волны")]
    [SerializeField] private WaveConfig waveConfig;

    [Header("Параметр босса - кол-во врагов до оглуения")]
    [SerializeField] private int killsForBossEvent = 20;

    
    private Pool_Enemy _pool;
    private int _waveIndex = -1;
    private int _alive = 0;
    private int _killsSinceBoss = 0;
    private int _bossMinionsAlive = 0;

   
    private System.Action _onAllMinionsDead;
    private System.Action _onBossKillsReached;

    private void Awake()
    {
        _pool = GetComponent<Pool_Enemy>();
        if (spawnCenter == null) spawnCenter = transform;
        if (waveConfig == null) { enabled = false; return; }
        _pool.PrewarmFromWaveConfig(waveConfig);
    }

    private void Start() => StartCoroutine(RunWaves());

    //Основной цикл волн
    private IEnumerator RunWaves()
    {
        while (_waveIndex < waveConfig.waves.Count - 1)
        {
            yield return new WaitUntil(() => _alive == 0);
            if (_waveIndex >= 0) yield return new WaitForSeconds(waveConfig.timeBetweenWaves);

            _waveIndex++;
            SpawnWave(waveConfig.waves[_waveIndex]);
        }
        yield return new WaitUntil(() => _alive == 0);
    }

    
    private void SpawnWave(WaveData wave)
    {
        var toSpawn = new List<EnemyAbstract>();
        foreach (var r in wave.enemies)
        {
            if (r.prefab == null) continue;
            int cnt = Random.Range(r.min, r.max + 1);
            for (int i = 0; i < cnt; i++) toSpawn.Add(r.prefab);
        }

        _alive += toSpawn.Count;
        if (toSpawn.Count == 0) return;

        //Сортировка по приоритету врагов (кто будет первый выходить) - добавил для большей гибкости в настройке волн
        toSpawn = toSpawn.OrderBy(e => wave.enemies.First(x => x.prefab == e).priority).ToList();

        
        StartCoroutine(SpawnWaveCoroutine(toSpawn));
    }

    private IEnumerator SpawnWaveCoroutine(List<EnemyAbstract> enemies)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            Spawn(enemies[i], true);
            if (i < enemies.Count - 1)
                yield return new WaitForSeconds(waveConfig.timeBetweenSpawns);
        }
    }

    // Босс атакует - задаем параетры для его волны
    public Coroutine StartBossAttack(
        float interval,
        System.Action onAllMinionsDead = null,
        params (EnemyAbstract prefab, int min, int max, int priority)[] enemies)
    {
        _onAllMinionsDead = onAllMinionsDead;
        _killsSinceBoss = 0;
        _bossMinionsAlive = 0;

        return StartCoroutine(BossCoroutine(interval, enemies));
    }

    private IEnumerator BossCoroutine(
        float interval,
        (EnemyAbstract prefab, int min, int max, int priority)[] enemies)
    {
        if (enemies is not { Length: > 0 })
        {
            _onAllMinionsDead?.Invoke();
            yield break;
        }

        var list = GenerateEnemyList(enemies);
        if (list.Count == 0)
        {
            _onAllMinionsDead?.Invoke();
            yield break;
        }

        _bossMinionsAlive = list.Count;
        list = list.OrderBy(e => enemies.First(x => x.prefab == e).priority).ToList();

        for (int i = 0; i < list.Count; i++)
        {
            SpawnBossMinion(list[i]);
            if (i < list.Count - 1)
                yield return new WaitForSeconds(interval);
        }
    }

    private void SpawnBossMinion(EnemyAbstract prefab)
    {
        var enemy = _pool.GetEnemy(prefab);
        if (!enemy) return;

        float x = Random.Range(-lineLength * 0.5f, lineLength * 0.5f);
        enemy.transform.position = spawnCenter.position + new Vector3(x, 0, 0);
        enemy.transform.rotation = Quaternion.identity;
        enemy.currentHP = enemy.maxHP;
        enemy.currentSpeed = enemy.speed;

        enemy.OnDeath = null;
        enemy.OnDeath += OnBossMinionDeath;
    }

    private void OnBossMinionDeath(EnemyAbstract deadEnemy)
    {
        _killsSinceBoss++;
        _bossMinionsAlive--;

        if (_bossMinionsAlive <= 0)
            _onAllMinionsDead?.Invoke();

        if (_killsSinceBoss >= killsForBossEvent)
        {
            _onBossKillsReached?.Invoke();
            _killsSinceBoss = 0; 
        }

        _pool.ReturnEnemy(deadEnemy);
    }

    //Спавн для обычных волн
    private void Spawn(EnemyAbstract prefab, bool track)
    {
        var e = _pool.GetEnemy(prefab);
        if (!e) { if (track) _alive--; return; }

        float x = Random.Range(-lineLength * 0.5f, lineLength * 0.5f);
        e.transform.position = spawnCenter.position + new Vector3(x, 0, 0);
        e.transform.rotation = Quaternion.identity;
        e.currentHP = e.maxHP;
        e.currentSpeed = e.speed;

        e.OnDeath = null;
        if (track)
            e.OnDeath += OnWaveEnemyDeath;
        else
            e.OnDeath += enemy => _pool.ReturnEnemy(enemy);
    }

    private void OnWaveEnemyDeath(EnemyAbstract deadEnemy)
    {
        _alive--;
        _pool.ReturnEnemy(deadEnemy);
    }

   
    private List<EnemyAbstract> GenerateEnemyList((EnemyAbstract prefab, int min, int max, int priority)[] enemies)
    {
        var list = new List<EnemyAbstract>();
        foreach (var (prefab, min, max, _) in enemies)
        {
            if (prefab == null) continue;
            int count = Random.Range(min, max + 1);
            for (int i = 0; i < count; i++) list.Add(prefab);
        }
        return list;
    }

    //Подписка на событие N убийств
    public void SubscribeToBossKillsEvent(System.Action callback) => _onBossKillsReached += callback;
    public void UnsubscribeFromBossKillsEvent(System.Action callback) => _onBossKillsReached -= callback;
}