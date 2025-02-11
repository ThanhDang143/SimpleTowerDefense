using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    #region TowerDefense
    [Space]
    [SerializeField] private Image imgHealth;

    private Dictionary<Stat, float> baseStats;
    private Dictionary<Stat, float> curStats;
    private int reward;

    MapCell nextCell;
    private float tempHP;

    public void Setup(EnemyInWave enemyData)
    {
        GameManager.Instance.AddEnemy(this);
        nextCell = movePath[moveIndex];

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

        float distance = Vector3.Distance(transform.position, nextCell.GetWorldPos());
        if (distance < 0.1f)
        {
            OnReachTarget();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, nextCell.GetWorldPos(), Time.deltaTime * GetStat(Stat.MOVE_SPEED));
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

    private void OnReachTarget()
    {
        moveIndex++;

        // Reach end point
        if (moveIndex >= movePath.Count)
        {
            OnReachEndPoint();
            return;
        }

        nextCell = movePath[moveIndex];
    }

    public void OnReachEndPoint()
    {
        GameManager.Instance.AddResources(ResourcesKey.LEVEL_HP, -GetStat(Stat.DMG));
        OnDead();
    }
    #endregion

    #region PathFinding

    private List<MapCell> movePath;
    private int moveIndex = 0;

    public void Setup(EnemyInWave enemyData, MapCell startCell, MapCell endCell)
    {
        moveIndex = 0;
        movePath = PathFinding.Instance.FindPath(startCell, endCell);
        if (movePath == null)
        {
            Debug.Log("<color=red>Cannot find way to target</color>");
            OnDead();
            return;
        }

        if (!IsValidPath())
        {
            if (GameManager.Instance.IsCanShowDebug())
            {
                foreach (MapCell mapCell in movePath)
                {
                    mapCell.SetTempBGColor(Color.red);
                }
            }
            OnDead();
            return;
        }
        else
        {
            if (GameManager.Instance.IsCanShowDebug())
            {
                foreach (MapCell mapCell in movePath)
                {
                    mapCell.SetTempBGColor(Color.green);
                }
            }
        }

        EditorApplication.isPaused = true;
        Setup(enemyData);
    }

    public bool IsValidPath()
    {
        if (movePath == null || movePath.Count == 0) return false;

        if (movePath.Count < 2) return true;

        int sectionCount = 0;
        for (int i = 0; i < movePath.Count - 2; i++)
        {
            MapCell curCell = movePath[i];
            MapCell nextCell = movePath[i + 2];
            if (curCell.GetGridPos().x != nextCell.GetGridPos().x && curCell.GetGridPos().y != nextCell.GetGridPos().y)
            {
                sectionCount++;
            }
        }

        return sectionCount <= 2;
    }

    #endregion
}
