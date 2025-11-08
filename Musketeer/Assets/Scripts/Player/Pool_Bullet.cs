using UnityEngine;
using System.Collections.Generic;

public class Pool_Bullet : MonoBehaviour
{
    [Header("Настройки пула")]
    public GameObject bulletPrefab;
    public int poolSize = 20;

    private List<GameObject> bullets = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform);
            bullet.SetActive(false);
            bullets.Add(bullet);
        }
    }

    public GameObject GetBullet()
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            if (!bullets[i].activeInHierarchy)
                return bullets[i];
        }

        return null;
    }
}
