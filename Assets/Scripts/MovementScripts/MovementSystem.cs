using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    private BFSResult movementRange = new BFSResult();
    private List<Vector3Int> currentPath = new List<Vector3Int>();

    Dictionary<Vector3Int, Hex> hexTileDict = new Dictionary<Vector3Int, Hex>();


    public void HideRange(HexGrid hexGrid)
    {
        foreach (Vector3Int hexPosition in movementRange.GetRangePositions())
        {
            hexGrid.GetTileAt(hexPosition).DisableHighlight();
        }
        movementRange = new BFSResult();
    }

    public void ShowRange(Unit selectedUnit, HexGrid hexGrid)
    {
        CalcualteRange(selectedUnit, hexGrid);

        hexTileDict = hexGrid.GetHexTileDict();

        foreach(var hex in hexTileDict)
        {
            Debug.Log("Hex key: " + hex.Key + "Hex Value: " + hex.Value);
        }

        Vector3Int unitPos = hexGrid.GetClosestHex(selectedUnit.transform.position);


        foreach (Vector3Int hexPosition in movementRange.GetRangePositions())
        {
            //Debug.Log("SHOW RANGE hex position is " + hexPosition);
            if (unitPos == hexPosition)
                continue;

            hexGrid.GetTileAt(hexPosition).EnableHighlight();
        }
    }

    public void CalcualteRange(Unit selectedUnit, HexGrid hexGrid)
    {
        movementRange = GraphSearch.BFSGetRange(hexGrid, hexGrid.GetClosestHex(selectedUnit.transform.position), selectedUnit.MovementPoints);
    }


    public void ShowPath(Vector3Int selectedHexPosition, HexGrid hexGrid)
    {
        if (movementRange.GetRangePositions().Contains(selectedHexPosition))
        {
            foreach (Vector3Int hexPosition in currentPath)
            {
                hexGrid.GetTileAt(hexPosition).ResetHighlight();
            }
            currentPath = movementRange.GetPathTo(selectedHexPosition);
            foreach (Vector3Int hexPosition in currentPath)
            {
               // Debug.Log("CURRENT PATH HEX IS " + hexPosition);
                hexGrid.GetTileAt(hexPosition).HighlightPath();
            }
        }
    }


    public int CalculateEnemyPathDistance(Vector3Int selectedEnemyHexPosition, HexGrid hexGrid)
    {
        if (movementRange.GetRangePositions().Contains(selectedEnemyHexPosition))
        {
            currentPath = movementRange.GetPathTo(selectedEnemyHexPosition);
            return currentPath.Count;
        }

        else
        {
            return 100;
        }
    }




    public void MoveUnit(Unit selectedUnit, HexGrid hexGrid)
    {
        //Debug.Log("Moving unit " + selectedUnit.name);
        selectedUnit.MoveThroughPath(currentPath.Select(pos => hexGrid.GetTileAt(pos).transform.position).ToList(), selectedUnit);

    }

    public bool IsHexInRange(Vector3Int hexPosition)
    {
        return movementRange.IsHexPositionInRange(hexPosition);
    }
}