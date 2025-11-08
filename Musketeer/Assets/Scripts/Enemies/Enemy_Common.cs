using UnityEngine;

public class Enemy_Common : EnemyAbstract
{
    private void Update()
    {
        if (playerTarget != null)
        {
            MoveToPlayer(playerTarget, currentSpeed);
        }
    }
}
