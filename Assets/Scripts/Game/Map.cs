using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class Map : SerializedScriptableObject
{
    [PropertyOrder(0)] public string mapName = "Map_00";
    [PropertyOrder(0), MinValue(1), MaxValue(30), OnValueChanged("OnChangeSize")] public Vector2Int gridSize = new Vector2Int(20, 20);
    [PropertyOrder(0), MinValue(1f), MaxValue(10f)] public float cellSize = 10f;

    [PropertyOrder(2), OnInspectorInit("OnInit"), TableMatrix(HideColumnIndices = true, HideRowIndices = true, DrawElementMethod = "DrawColoredEnumElement", ResizableColumns = false, SquareCells = true, RowHeight = 20)]
    public MapCellType[,] view;

    public MapCellType GetValue(int x, int y)
    {
        return view[x, view.GetLength(1) - y - 1];
    }

#if UNITY_EDITOR
    [EnumToggleButtons, PropertyOrder(1)] public MapCellType choosingCellType = MapCellType.EMPTY;

    [PropertyOrder(3), Button("Reset Map", ButtonSizes.Large), GUIColor(1, 0, 0)]
    private void ResetMap()
    {
        view = new MapCellType[gridSize.x, gridSize.y];
        Debug.Log("Map reset!");
    }

    private MapCellType DrawColoredEnumElement(Rect rect, MapCellType value)
    {
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            value = choosingCellType;
            GUI.changed = true;
            Event.current.Use();
        }

        EditorGUI.DrawRect(rect.Padding(0.5f), Utilities.ChooseColor(value));

        return value;
    }

    public void OnInit()
    {
        view ??= new MapCellType[gridSize.x, gridSize.y];
    }

    private void OnChangeSize()
    {
        MapCellType[,] newView = new MapCellType[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (x < view.GetLength(0) && y < view.GetLength(1))
                {
                    newView[x, y] = view[x, y];
                }
                else
                {
                    newView[x, y] = MapCellType.EMPTY;
                }
            }
        }

        view = newView;
    }
#endif
}
