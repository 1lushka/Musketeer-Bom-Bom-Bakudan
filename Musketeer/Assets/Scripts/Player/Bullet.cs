using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Параметры снаряда")]
    public float speed = 20f;
    public float damage = 10f;
    public float lifetime = 5f;

    private Vector3 moveDirection;

    public void Shoot(Vector3 target)
    {
        Vector3 flatTarget = new Vector3(target.x, transform.position.y, target.z);
        moveDirection = (flatTarget - transform.position).normalized;

        gameObject.SetActive(true);

        CancelInvoke();
        Invoke(nameof(Deactivate), lifetime);
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyAbstract enemy = other.GetComponent<EnemyAbstract>();
        if (enemy == null)
            enemy = other.GetComponentInParent<EnemyAbstract>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Deactivate();
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
