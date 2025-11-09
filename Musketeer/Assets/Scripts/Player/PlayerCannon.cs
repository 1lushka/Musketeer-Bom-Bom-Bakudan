using UnityEngine;
using System.Collections;

public class PlayerCannon : MonoBehaviour
{
    [Header("Параметры стрельбы")]
    public float shootCooldown = 0.5f;
    public Transform shootPoint;

    [Header("Пул снарядов")]
    public Pool_Bullet bulletPool;

    [Header("UI и состояния")]
    public LevelUI levelUI; 

    private bool canShoot = true;

    private void Start()
    {
        levelUI?.SetReady();
    }

    public void Shoot(Vector3 target)
    {
        if (!canShoot) return;

        GameObject bullet = bulletPool.GetBullet();
        if (bullet == null) return;

        bullet.transform.position = shootPoint.position;
        bullet.GetComponent<Bullet>().Shoot(target);

        StartCoroutine(CooldownTimer());
    }

    IEnumerator CooldownTimer()
    {
        canShoot = false;
        levelUI?.SetCooldownStart();

        float timer = shootCooldown;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            levelUI?.SetCooldownProgress(1f - (timer / shootCooldown));
            yield return null;
        }

        canShoot = true;
        levelUI?.SetReady();
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyAbstract enemy = other.GetComponent<EnemyAbstract>();
        if (enemy == null)
            enemy = other.GetComponentInParent<EnemyAbstract>();

        if (enemy != null)
        {
            // gameOverCanvas.Activate();
        }
    }
}
