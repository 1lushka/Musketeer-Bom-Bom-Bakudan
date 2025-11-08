using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Параметры снаряда")]
    public float speed = 20f;
    public float damage = 10f;
    public float lifetime = 5f;

    private Vector3 targetPosition;
    private float spawnHeight;

    public void Shoot(Vector3 target)
    {
        spawnHeight = transform.position.y;
        targetPosition = new Vector3(target.x, spawnHeight, target.z);
        gameObject.SetActive(true);
        CancelInvoke();
        Invoke(nameof(Deactivate), lifetime);
    }

    private void Update()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyAbstract enemy = other.GetComponent<EnemyAbstract>();
        if (enemy == null) enemy = other.GetComponentInParent<EnemyAbstract>();
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
