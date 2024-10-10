using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCell
{
    private Grid<MapCell> grid;
    private Vector2Int posInGrid;
    private MapCellType cellType;
    private MapCellBG bgView;
    private TowerController tower;

    public MapCell(Grid<MapCell> _grid, int x, int y, MapCellType _cellType)
    {
        // Set data
        grid = _grid;
        posInGrid = new Vector2Int(x, y);
        cellType = _cellType;

        // Create View
        bgView = GameObject.Instantiate(GameManager.Instance.GetMapCellBGPrefab(), grid.GetWorldPosition(x, y) + new Vector3(grid.GetCellSize(), grid.GetCellSize()) * 0.5f, Quaternion.identity, GameManager.Instance.GetGameContainer());
        bgView.name = "MapCellBG_" + x + "_" + y;
        bgView.Setup(grid.GetCellSize(), Utilities.ChooseColor(cellType));

        // Check special cell
        if (cellType == MapCellType.START_POINT) GameManager.Instance.SetStartCell(this);
        if (cellType == MapCellType.END_POINT) GameManager.Instance.SetEndCell(this);
    }

    public Vector3 GetWorldPosition()
    {
        return grid.GetWorldPosition(posInGrid.x, posInGrid.y) + new Vector3(grid.GetCellSize(), grid.GetCellSize()) * 0.5f;
    }

    public MapCellType GetCellType()
    {
        return cellType;
    }

    public void GetCellAround(out MapCell up, out MapCell right, out MapCell down, out MapCell left)
    {
        up = grid.GetValue(posInGrid.x, posInGrid.y + 1);
        right = grid.GetValue(posInGrid.x + 1, posInGrid.y);
        down = grid.GetValue(posInGrid.x, posInGrid.y - 1);
        left = grid.GetValue(posInGrid.x - 1, posInGrid.y);
    }

    public MapCellBG GetCellBG()
    {
        return bgView;
    }

    public bool IsCanMove()
    {
        return cellType == MapCellType.ROAD || cellType == MapCellType.START_POINT || cellType == MapCellType.END_POINT;
    }

    public bool IsCanPlaceTower()
    {
        return tower == null ? cellType == MapCellType.EMPTY : (cellType == MapCellType.EMPTY && !tower.IsMaxLevel());
    }

    public void PlaceTower(TowerController _tower)
    {
        tower = _tower;
    }

    public void RemoveTower()
    {
        if (tower == null) return;
        
        tower.OnSell();
        tower = null;
    }

    public TowerController GetTower()
    {
        return tower;
    }



    public override string ToString()
    {
        return cellType.ToString() + "\n" + posInGrid;
    }
}
