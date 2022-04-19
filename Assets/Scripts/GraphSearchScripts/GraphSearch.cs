using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphSearch
{
    // Bread First Search For Calculate The Cost
    public static BFSResult BFSGetRange(HexGrid hexGrid, Vector3Int startPoint, int movementPoints)
    {
        Dictionary<Vector3Int, Vector3Int?> visitedNodes = new Dictionary<Vector3Int, Vector3Int?>();
        Dictionary<Vector3Int, int> costSoFar = new Dictionary<Vector3Int, int>();
        Queue<Vector3Int> nodesToVisitQueue = new Queue<Vector3Int>();

        nodesToVisitQueue.Enqueue(startPoint);
        costSoFar.Add(startPoint, 0);
        visitedNodes.Add(startPoint, null);

        while (nodesToVisitQueue.Count > 0)
        {
            Vector3Int currentNode = nodesToVisitQueue.Dequeue();

            foreach (Vector3Int neighbourPosition in hexGrid.GetNeighboursFor(currentNode))
            {
                if (hexGrid.GetTileAt(neighbourPosition).IsObstacle())
                    continue;

                int nodeCost = hexGrid.GetTileAt(neighbourPosition).GetCost();
                int currentCost = costSoFar[currentNode];
                int newCost = currentCost + nodeCost;

                if (newCost <= movementPoints)
                {
                    if (!visitedNodes.ContainsKey(neighbourPosition))
                    {
                        visitedNodes[neighbourPosition] = currentNode;
                        costSoFar[neighbourPosition] = newCost;
                        nodesToVisitQueue.Enqueue(neighbourPosition);

                    }

                    else if (costSoFar[neighbourPosition] > newCost)
                    {
                        costSoFar[neighbourPosition] = newCost;
                        visitedNodes[neighbourPosition] = currentNode;

                    }

                }


            }
        }

        return new BFSResult
        {
            visitedNodeDict = visitedNodes
        };

    }

    public static List<Vector3Int> GeneratePathBFS(Vector3Int current, Dictionary<Vector3Int, Vector3Int?> visitedNodeDict)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        path.Add(current);
        while (visitedNodeDict[current] != null)
        {
            path.Add(visitedNodeDict[current].Value);
            current = visitedNodeDict[current].Value;
        }

        path.Reverse();
        return path.Skip(1).ToList();

    }

}

public struct BFSResult
{
    public Dictionary<Vector3Int, Vector3Int?> visitedNodeDict;

    public List<Vector3Int> GetPathTo(Vector3Int destination)
    {
        if (visitedNodeDict.ContainsKey(destination) == false)
            return new List<Vector3Int>();
        return GraphSearch.GeneratePathBFS(destination, visitedNodeDict);
    }

    public bool IsHexPositionInRange(Vector3Int position)
    {
        return visitedNodeDict.ContainsKey(position);
    }

    public IEnumerable<Vector3Int> GetRangePositions()
        => visitedNodeDict.Keys;

}



