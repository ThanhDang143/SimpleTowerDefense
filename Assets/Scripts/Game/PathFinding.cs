using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class PathFinding : Singleton<PathFinding>
{
    public List<MapCell> FindPath(MapCell startCell, MapCell endCell)
    {
        Node startNode = new Node(startCell);
        Node endNode = new Node(endCell);

        CalculateCost(startNode, endNode, out float startGCost, out float startHCost, out float startFCost);
        startNode.gCost = startGCost;
        startNode.hCost = startHCost;
        startNode.fCost = startFCost;
        startNode.preNode = null;

        List<Node> openList = new List<Node>() { startNode };
        List<Node> closeList = new List<Node>();

        while (openList.Count > 0)
        {
            Node currentNode = GetLowestCostNode(openList);

            if (currentNode.cell == endNode.cell) return GetPath(currentNode);

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            List<Node> neighbourNodes = GetNeighbourNodes(currentNode, openList, closeList);

            foreach (Node neighbourNode in neighbourNodes)
            {
                if (closeList.Contains(neighbourNode)) continue;

                float tentativeGCost = currentNode.gCost + currentNode.cell.GetCellSize();
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateHCost(neighbourNode, endNode);
                    neighbourNode.preNode = currentNode;
                    neighbourNode.fCost = neighbourNode.gCost + neighbourNode.hCost;
                }

                if (!openList.Contains(neighbourNode))
                {
                    openList.Add(neighbourNode);
                    neighbourNode.cell.SetTempBGColor(Color.yellow);
                }
            }
        }

        // Cannot find target after scan all map
        return null;
    }

    public List<MapCell> GetPath(Node endNode)
    {
        List<MapCell> result = new List<MapCell>();

        Node curNode = endNode;
        while (curNode.preNode != null)
        {
            result.Add(curNode.cell);
            curNode = curNode.preNode;
        }
        result.Reverse();

        return result;
    }

    private List<Node> GetNeighbourNodes(Node currentNode, List<Node> openList, List<Node> closedList)
    {
        currentNode.cell.GetCellAround(out MapCell up, out MapCell right, out MapCell down, out MapCell left);
        List<Node> neighbourCells = new List<Node>();

        AddNode(up);
        AddNode(right);
        AddNode(down);
        AddNode(left);

        return neighbourCells;

        void AddNode(MapCell cell)
        {
            if (cell != null && !cell.IsObstacle()) neighbourCells.Add(GetNode(cell, openList, closedList));
        }
    }

    private Node GetNode(MapCell cell, List<Node> openList, List<Node> closedList)
    {
        if (openList.Any(n => n.cell == cell))
        {
            return openList.First(n => n.cell == cell);
        }

        if (closedList.Any(n => n.cell == cell))
        {
            return closedList.First(n => n.cell == cell);
        }

        Node newNode = new Node(cell);
        newNode.gCost = float.MaxValue;
        newNode.hCost = 0;
        newNode.fCost = newNode.gCost + newNode.hCost;
        newNode.preNode = null;

        return newNode;
    }

    private Node GetLowestCostNode(List<Node> pathNodes)
    {
        Node lowestCostNode = pathNodes[0];
        for (int i = 1; i < pathNodes.Count; i++)
        {
            if (pathNodes[i].fCost < lowestCostNode.fCost)
            {
                lowestCostNode = pathNodes[i];
            }
        }

        return lowestCostNode;
    }

    private void CalculateCost(Node thisNode, Node endNode, out float gCost, out float hCost, out float fCost)
    {
        gCost = thisNode.preNode != null ? thisNode.preNode.gCost + thisNode.cell.GetCellSize() : 0;
        hCost = CalculateHCost(thisNode, endNode);
        fCost = gCost + hCost;
    }

    private float CalculateHCost(Node thisNode, Node endNode)
    {
        return Mathf.Abs(thisNode.cell.GetGridPos().x - endNode.cell.GetGridPos().x) + Mathf.Abs(thisNode.cell.GetGridPos().y - endNode.cell.GetGridPos().y);
    }

}

public class Node
{
    public float gCost;
    public float hCost;
    public float fCost;
    public MapCell cell;
    public Node preNode;

    public Node(MapCell _cell)
    {
        cell = _cell;
    }
}
