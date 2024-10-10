using System;
using UnityEngine;

public class Grid<T>
{
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPos;
    private T[,] gridData;

    public Grid(int _width, int _height, float _cellSize, Vector3 _originPos, Func<Grid<T>, int, int, T> createGridData)
    {
        width = _width;
        height = _height;
        cellSize = _cellSize;
        originPos = _originPos;

        gridData = new T[_width, _height];

        for (int x = 0; x < gridData.GetLength(0); x++)
        {
            for (int y = 0; y < gridData.GetLength(1); y++)
            {
                gridData[x, y] = createGridData(this, x, y);
            }
        }

        // Debug View
        if (!GameManager.IsInstanceNull() && GameManager.Instance.IsCanShowDebug())
        {
            TextMesh[,] debugTextArray = new TextMesh[_width, _height];
            debugTextArray ??= new TextMesh[_width, _height];
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    debugTextArray[i, j] = Utilities.CreateWorldText(gridData[i, j]?.ToString(), GameManager.Instance.GetGameContainer(), GetWorldPosition(i, j) + new Vector3(cellSize, cellSize) * 0.5f, 20, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center, 5000);
                    if (gridData[i, j] is MapCell cellData)
                    {
                        debugTextArray[i, j].color = Utilities.ChooseColor(cellData.GetCellType());
                    }
                }
            }

            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) =>
            {
                debugTextArray[eventArgs.x, eventArgs.y].text = gridData[eventArgs.x, eventArgs.y]?.ToString();
            };
        }
    }

    #region Get Data
    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPos;
    }

    public void GetXY(Vector3 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPos - originPos).x / cellSize);
        y = Mathf.FloorToInt((worldPos - originPos).y / cellSize);
    }

    public T GetValue(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            Debug.Log("<color=red>Out of Grid</color>");
            return default;
        }

        return gridData[x, y];
    }

    public T GetValue(Vector3 worldPos)
    {
        GetXY(worldPos, out int x, out int y);
        return GetValue(x, y);
    }

    #endregion

    #region Set Data
    public void SetValue(int x, int y, T value)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            Debug.Log("<color=red>Out of Grid</color>");
            return;
        }

        gridData[x, y] = value;
        TriggerGridObjectChanged(x, y);
    }

    public void SetValue(Vector3 worldPos, T value)
    {
        GetXY(worldPos, out int x, out int y);
        SetValue(x, y, value);
    }
    #endregion

    # region Trigger

    public void TriggerGridObjectChanged(int x, int y)
    {
        if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
    }

    #endregion
}
