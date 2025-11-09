using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Wave Config", menuName = "Wave System/Wave Config")]
public class WaveConfig : ScriptableObject
{
    [Header("Общие настройки")]
    public float timeBetweenWaves = 8f;
    public float timeBetweenSpawns = 0.4f;

    [Header("Настройка волн")]
    public List<WaveData> waves = new List<WaveData>();
}

[System.Serializable]
public class WaveData
{
    public List<EnemySpawnRule> enemies = new List<EnemySpawnRule>();
}

[System.Serializable]
public class EnemySpawnRule
{
    public EnemyAbstract prefab;

    [Tooltip("От скольки(включительно)")]
    public int min = 5;

    [Tooltip("До скольки (включительно)")]
    public int max = 10;

    [Range(1, 100)]
    [Tooltip("Чем выше — тем раньше спавнится (приоритет)")]
    public int priority = 50;
}