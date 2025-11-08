using UnityEngine;

public abstract class EnemyAbstract : MonoBehaviour
{
    [Header("Характеристики врага")]
    public float maxHP = 100f;
    public float currentHP;
    public float speed = 3f;
    public float currentSpeed;

    protected Transform playerTarget;

    protected virtual void Start()
    {
        playerTarget = FindAnyObjectByType<PlayerCannon>().transform;
        currentHP = maxHP;
        currentSpeed = speed;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            KillEnemy();
        }
    }

    protected virtual void KillEnemy()
    {
        gameObject.SetActive(false);
    }

    public virtual void MoveToPlayer(Transform target, float currentSpeed)
    {
        if (target == null) return;

        playerTarget = target;
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            currentSpeed * Time.deltaTime
        );
    }
}
