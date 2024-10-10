using System.Collections;
using System.Collections.Generic;
using BakingSheetImpl;
using UnityEngine;

public class TowerPreview : MonoBehaviour
{
    [Space]
    [SerializeField] private SpriteRenderer srTowerIcon;
    [SerializeField] private SpriteRenderer srAtkZone;

    private Vector3 mousePos = Vector3.zero;

    private Tower towerData;
    private MapCell mouseOverlapCell;

    public void Setup(Tower _towerData)
    {
        towerData = _towerData;
        LoadTowerIcon();
    }

    private void LoadTowerIcon()
    {
        srTowerIcon.sprite = towerData.Icon.Get<Sprite>();
    }

    public void ManualUpdate()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseOverlapCell = GameManager.Instance.GetCell(mousePos);

        FollowMouse();
        ShowAtkZone();
    }

    private void FollowMouse()
    {
        mousePos.z = 1f;
        transform.position = mousePos;
    }

    public void ShowAtkZone()
    {
        if (!GameManager.Instance.IsCanPlaceTower())
        {
            SetAtkZone(new Color(1, 0, 0, 0.5f), towerData.GetStat(Stat.ATK_RANGE) * towerData.Upgrades[0].PowerMultiplier);
            return;
        }

        TowerController tower = mouseOverlapCell.GetTower();
        int level = tower == null ? 0 : tower.GetLevel() + 1;
        float atkRange = towerData.GetStat(Stat.ATK_RANGE) * towerData.Upgrades[Mathf.Clamp(level, 0, towerData.Upgrades.Count - 1)].PowerMultiplier;
        SetAtkZone(new Color(0, 0, 1, 0.5f), atkRange);
    }

    private void SetAtkZone(Color color, float size)
    {
        srAtkZone.color = color;
        srAtkZone.transform.localScale = Vector3.one * size * 2f;
    }

    public string GetTowerID()
    {
        return towerData.Id;
    }

    public Tower GetTowerInfo()
    {
        return towerData;
    }
}
