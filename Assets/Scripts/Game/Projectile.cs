using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Space]
    [SerializeField] private float colliderSize = 0.1f;

    private EnemyController target;
    private float damage;

    public void Setup(EnemyController _target, float dmg)
    {
        target = _target;
        damage = dmg;

        target.ProjectileLockOn(dmg);
        GameManager.Instance.AddProjectile(this);
    }

    private void Destroy()
    {
        GameManager.Instance.RemoveProjectile(this);
        Destroy(gameObject);
    }

    public void ManualUpdate()
    {
        if (target == null)
        {
            Destroy();
            return;
        }

        if (IsCollideWithTarget())
        {
            OnReachTarget();
        }
        else
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * target.GetStat(Stat.MOVE_SPEED) * 5f);
    }

    private bool IsCollideWithTarget()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, colliderSize);
        if (collider != null && collider.transform.parent.TryGetComponent(out EnemyController enemy) && enemy == target)
        {
            return true;
        }

        return false;
    }

    private void OnReachTarget()
    {
        target?.TakeDamage(damage);
        Destroy();
    }

    #region Debug

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, colliderSize);
    }

    #endregion
}
