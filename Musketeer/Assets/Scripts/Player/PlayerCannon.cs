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
    //public GameOverCanvas gameOverCanvas;
    //public UIManager uiManager;

    private bool canShoot = true;

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
        float timer = shootCooldown;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            //uiManager.UpdateCooldownUI(timer / shootCooldown);
            yield return null;
        }

        canShoot = true;
        //uiManager.UpdateCooldownUI(0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyAbstract enemy = other.GetComponent<EnemyAbstract>();
        if (enemy == null) enemy = other.GetComponentInParent<EnemyAbstract>();
        if (enemy != null)
        {
            //gameOverCanvas.Activate();
        }
    }
}
