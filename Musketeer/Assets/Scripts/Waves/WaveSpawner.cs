using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Pool_Enemy))]
public class WaveSpawner : MonoBehaviour
{
    public Transform spawnCenter;
    public float lineLength = 30f;
    public WaveConfig waveConfig;

    private Pool_Enemy _pool;
    private int _waveIndex = -1;
    private int _alive = 0;

    private void Awake()
    {
        _pool = GetComponent<Pool_Enemy>();
        if (spawnCenter == null) spawnCenter = transform;
        if (waveConfig == null) { enabled = false; return; }
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
            if (_waveIndex >= 0) yield return new WaitForSeconds(waveConfig.timeBetweenWaves);
            _waveIndex++;
            var wave = waveConfig.waves[_waveIndex];
            var toSpawn = new List<EnemyAbstract>();
            foreach (var r in wave.enemies)
            {
                if (r.prefab == null) continue;
                int cnt = Random.Range(r.min, r.max + 1);
                for (int i = 0; i < cnt; i++) toSpawn.Add(r.prefab);
            }
            _alive = toSpawn.Count;
            if (_alive == 0) continue;
            toSpawn = toSpawn.OrderBy(e => wave.enemies.First(x => x.prefab == e).priority).ToList();
            foreach (var p in toSpawn)
            {
                Spawn(p, true);
                yield return new WaitForSeconds(waveConfig.timeBetweenSpawns);
            }
        }
        yield return new WaitUntil(() => _alive == 0);
    }

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
        if (track) e.OnDeath += _ => { _alive--; _pool.ReturnEnemy(e); };
    }

    public Coroutine StartBossAttack(
        float interval,
        System.Action onComplete = null,
        params (EnemyAbstract prefab, int min, int max, int priority)[] enemies)
    {
        return StartCoroutine(BossCoroutine(interval, onComplete, enemies));
    }

    private IEnumerator BossCoroutine(
        float interval,
        System.Action onComplete,
        (EnemyAbstract prefab, int min, int max, int priority)[] enemies)
    {
        if (enemies == null || enemies.Length == 0)
        {
            onComplete?.Invoke();
            yield break;
        }

        var list = new List<EnemyAbstract>();

        foreach (var (prefab, min, max, _) in enemies)
        {
            if (prefab == null) continue;
            int count = Random.Range(min, max + 1);
            for (int i = 0; i < count; i++) list.Add(prefab);
        }

        if (list.Count == 0)
        {
            onComplete?.Invoke();
            yield break;
        }

        list = list.OrderBy(e => enemies.First(x => x.prefab == e).priority).ToList();

        for (int i = 0; i < list.Count; i++)
        {
            Spawn(list[i], false);
            if (i < list.Count - 1) yield return new WaitForSeconds(interval);
        }

        onComplete?.Invoke();
    }
}