using System.Collections;
using System.Collections.Generic;
using AddressableImpl;
using BakingSheetImpl;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    [Space]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Tower towerData;
    private Dictionary<Stat, float> stats = new Dictionary<Stat, float>();
    private int level = 0;
    private Projectile projectilePrefab;
    private float attackTimer = 0f;
    private EnemyController target;

    public void Setup(Tower _towerData, int _level = 0)
    {
        towerData = _towerData;
        level = _level;

        GameManager.Instance.AddTower(this);
        transform.localScale = Vector3.one / GameManager.Instance.GetCellSize();

        LoadView();
        LoadStats();
        LoadProjectile();
    }

    private void LoadView()
    {
        spriteRenderer.sprite = towerData.Upgrades[level].Icon.Get<Sprite>();
    }

    private void LoadStats()
    {
        stats = GameManager.Instance.ScaleStats(towerData.Stats, towerData.Upgrades[level].PowerMultiplier);
    }

    private void LoadProjectile()
    {
        AddressableManager.LoadAssetAsync<GameObject>(towerData.Upgrades[level].ProjectileAddress, (obj) =>
        {
            projectilePrefab = obj.GetComponent<Projectile>();
        });
    }

    public float GetStat(Stat stat)
    {
        if (!stats.ContainsKey(stat))
        {
            stats.Add(stat, 0);
        }

        return stats[stat];
    }

    public int GetLevel()
    {
        return level;
    }

    public void ManualUpdate()
    {
        Attack();
        LookAtTarget();
    }

    private void LookAtTarget()
    {
        if (target != null)
        {
            Vector3 direction = target.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void Attack()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= GetStat(Stat.ATK_SPEED))
        {
            FindTarget();

            if (target != null)
            {
                attackTimer = 0f;
                Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity, GameManager.Instance.GetGameContainer());
                projectile.Setup(target, GetStat(Stat.DMG));
            }
        }
    }

    public EnemyController FindTarget()
    {
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= GetStat(Stat.ATK_RANGE) && !target.IsWaitingToDie())
            {
                return target;
            }

            target = null;
        }

        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Collider2D targetCollider = Physics2D.OverlapCircle(transform.position, GetStat(Stat.ATK_RANGE), enemyLayer);
        if (targetCollider != null && targetCollider.transform.parent.TryGetComponent(out EnemyController enemy) && !enemy.IsWaitingToDie())
        {
            target = enemy;
        }

        return target;
    }

    public void Upgrade()
    {
        level++;
        Setup(towerData, level);
    }

    public bool IsMaxLevel()
    {
        return level >= towerData.Upgrades.Count - 1;
    }

    private float GetSellPrice()
    {
        float price = towerData.Upgrades[level].Price * 0.5f;
        return Mathf.RoundToInt(price);
    }

    public void OnSell()
    {
        GameManager.Instance.AddResources(ResourcesKey.COIN, GetSellPrice());
        GameManager.Instance.RemoveTower(this);
        Destroy(gameObject);
    }

    public string GetTowerID()
    {
        return towerData.Id;
    }

    #region Debug

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, GetStat(Stat.ATK_RANGE));
    }

    #endregion
}
