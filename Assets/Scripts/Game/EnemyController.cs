using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Space]
    [SerializeField] private Image imgHealth;

    private Dictionary<Stat, float> baseStats;
    private Dictionary<Stat, float> curStats;
    private int reward;

    private MapCell preCell;
    private MapCell nextCell;
    private float tempHP;

    public void Setup(EnemyInWave enemyData)
    {
        GameManager.Instance.AddEnemy(this);
        preCell = GameManager.Instance.GetStartCell();
        nextCell = GameManager.Instance.GetStartCell();

        reward = enemyData.Enemy.Ref.Reward;
        baseStats = GameManager.Instance.ScaleStats(enemyData.Enemy.Ref.Stats, enemyData.ScaleIndex);
        curStats = new Dictionary<Stat, float>(baseStats);
        SetStat(Stat.HP, GetBaseStat(Stat.HP));
        tempHP = GetStat(Stat.HP);
    }

    public void OnDead()
    {
        GameManager.Instance.RemoveEnemy(this);
        Destroy(gameObject);
    }

    private void OnBeKilled()
    {
        GameManager.Instance.AddResources(ResourcesKey.COIN, reward);
        OnDead();
    }

    public bool IsWaitingToDie()
    {
        return tempHP <= 0;
    }

    public void ProjectileLockOn(float dmg)
    {
        tempHP -= dmg;
        if (tempHP <= 0)
        {
            GetComponentInChildren<Collider2D>().gameObject.layer = LayerMask.GetMask("Default");
        }
    }

    public void ManualUpdate()
    {
        if (nextCell == null) return;

        float distance = Vector3.Distance(transform.position, nextCell.GetWorldPosition());
        if (distance < 0.1f)
        {
            OnReachTarget();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, nextCell.GetWorldPosition(), Time.deltaTime * GetStat(Stat.MOVE_SPEED));
        }
    }

    private float GetBaseStat(Stat stat)
    {
        if (!baseStats.ContainsKey(stat))
        {
            baseStats.Add(stat, 0);
        }

        return baseStats[stat];
    }

    public float GetStat(Stat stat)
    {
        if (!curStats.ContainsKey(stat))
        {
            curStats.Add(stat, 0);
        }

        return curStats[stat];
    }

    public void SetStat(Stat stat, float value)
    {
        if (!curStats.ContainsKey(stat))
        {
            curStats.Add(stat, 0);
        }

        curStats[stat] = value;
        if (stat == Stat.HP)
        {
            imgHealth.fillAmount = GetStat(Stat.HP) / GetBaseStat(Stat.HP);
        }
    }

    public void TakeDamage(float dmg)
    {
        SetStat(Stat.HP, GetStat(Stat.HP) - dmg);
        if (GetStat(Stat.HP) <= 0)
        {
            OnBeKilled();
        }
    }

    private MapCell GetCurCell()
    {
        return GameManager.Instance.GetCell(transform.position);
    }

    private MapCell FindNewTarget()
    {
        MapCell curCell = GetCurCell();
        curCell.GetCellAround(out MapCell up, out MapCell right, out MapCell down, out MapCell left);

        if (up != null && up.IsCanMove() && up != preCell) return up;
        if (right != null && right.IsCanMove() && right != preCell) return right;
        if (down != null && down.IsCanMove() && down != preCell) return down;
        if (left != null && left.IsCanMove() && left != preCell) return left;

        return null;
    }

    private void OnReachTarget()
    {
        MapCell newCell = FindNewTarget();

        // Reach end point
        if (newCell == null && GetCurCell().GetCellType() == MapCellType.END_POINT)
        {
            OnReachEndPoint();
            return;
        }

        if (newCell == null && GetCurCell().GetCellType() != MapCellType.END_POINT)
        {
            Debug.Log("Can't find new target. Please check your map!");
            return;
        }

        preCell = nextCell;
        nextCell = newCell;
    }

    public void OnReachEndPoint()
    {
        GameManager.Instance.AddResources(ResourcesKey.LEVEL_HP, -GetStat(Stat.DMG));
        OnDead();
    }
}
