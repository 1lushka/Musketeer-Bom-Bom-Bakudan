using System.Collections.Generic;
using UnityEngine;

public class Pool_Enemy : MonoBehaviour
{
    private readonly Dictionary<EnemyAbstract, Queue<EnemyAbstract>> _pools = new();

    [Header("Настройки")]
    public int defaultPoolSize = 20;

    public void PrewarmFromWaveConfig(WaveConfig config)
    {
        var seen = new HashSet<EnemyAbstract>();

        foreach (var wave in config.waves)
            foreach (var rule in wave.enemies)
                if (rule.prefab && seen.Add(rule.prefab))
                    if (!_pools.ContainsKey(rule.prefab))
                        CreatePool(rule.prefab, defaultPoolSize);
    }

    public EnemyAbstract GetEnemy(EnemyAbstract prefab)
    {
        if (!prefab) return null;

        if (!_pools.TryGetValue(prefab, out var q))
        {
            CreatePool(prefab, defaultPoolSize);
            q = _pools[prefab];
        }

        EnemyAbstract e = q.Count > 0 ? q.Dequeue() : Instantiate(prefab, transform);

        e.gameObject.SetActive(true);
        e.transform.SetParent(null);
        return e;
    }

    public void ReturnEnemy(EnemyAbstract enemy)
    {
        if (!enemy) return;

        var prefab = enemy.GetComponent<EnemyAbstract>();
        if (!prefab) prefab = enemy;

        if (!_pools.TryGetValue(prefab, out var q))
            CreatePool(prefab, 5);

        enemy.gameObject.SetActive(false);
        enemy.transform.SetParent(transform);
        enemy.transform.localPosition = Vector3.zero;
        _pools[prefab].Enqueue(enemy);
    }

    private void CreatePool(EnemyAbstract prefab, int size)
    {
        var q = new Queue<EnemyAbstract>();
        _pools[prefab] = q;
        for (int i = 0; i < size; i++)
        {
            var e = Instantiate(prefab, transform);
            e.gameObject.SetActive(false);
            q.Enqueue(e);
        }
    }
}