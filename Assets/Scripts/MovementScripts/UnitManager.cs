using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField]
    private HexGrid hexGrid;

    [SerializeField]
    private MovementSystem movementSystem;

    [SerializeField]
    private Unit selectedUnit;
    private Hex previouslySelectedHex;

    [SerializeField]
    private Enemy selectedEnemy;

    private int DistanceOfEnemy;
    public bool PlayersTurn { get; private set; } = true;

    public bool EnemySelected { get; private set; } = false;

    void Update()
    {
        if (this.selectedEnemy != null)
        {
            if (!CheckEnemyAlive(this.selectedEnemy))
            {

                ClearOldEnemySelection();
                ClearOldSelection();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && this.selectedUnit != null && this.selectedEnemy != null)
        {
            if (DistanceOfEnemy > this.selectedUnit.AttackRange)
            {
                Debug.Log("Cannot attack distance is  " + DistanceOfEnemy + " your range is " + this.selectedUnit.AttackRange);

            }
            else
            {
                this.selectedUnit.Attack();
                Debug.Log("I am attacking with " + this.selectedUnit.DamagePoints + " damage");
                this.selectedEnemy.DamageDealt(this.selectedUnit.DamagePoints);
                ClearOldEnemySelection();
                ClearOldSelection();
            }
        }            
    }

    public void HandleUnitSelected(GameObject unit)
    {
        if (PlayersTurn == false)
            return;

        Unit unitReference = unit.GetComponent<Unit>();

        if (CheckIfTheSameUnitSelected(unitReference))
            return;

        if (this.EnemySelected)
        {
            ClearOldEnemySelection();
        }

        PrepareUnitForMovement(unitReference);
    }

    private bool CheckIfTheSameUnitSelected(Unit unitReference)
    {
        if (this.selectedUnit == unitReference)
        {
            ClearOldSelection();
            if (this.EnemySelected)
            {
                ClearOldEnemySelection();
            }
            return true;
        }
        return false;
    }

    private bool CheckIfTheSameEnemySelected(Enemy enemyReference)
    {
        if (this.selectedEnemy == enemyReference)
        {
            ClearOldEnemySelection();
            ClearOldSelection();
            return true;
        }
        return false;
    }


    public void HandleTerrainSelected(GameObject hexGO)
    {
        if (selectedUnit == null || PlayersTurn == false)
        {
            return;
        }

        Hex selectedHex = hexGO.GetComponent<Hex>();

        if (HandleHexOutOfRange(selectedHex.HexCoords) || HandleSelectedHexIsUnitHex(selectedHex.HexCoords))
            return;

        HandleTargetHexSelected(selectedHex);

    }


    public void HandleEnemySelected(GameObject enemy)
    {
        if (selectedUnit == null || PlayersTurn == false)
        {
            return;
        }
        Enemy enemyReference = enemy.GetComponent<Enemy>();

        if (CheckIfTheSameEnemySelected(enemyReference) || HandleEnemyOutOfRange(enemyReference.GetEnemyHexCoords()) || !CheckEnemyAlive(enemyReference))
            return;

        HandleTargetEnemySelected(enemyReference);

    }

    public bool CheckEnemyAlive(Enemy enemyReference)
    {
        return enemyReference.isAlive;
    }

    private void HandleTargetEnemySelected(Enemy enemyReference)
    {
        if (this.EnemySelected)
        {
            this.selectedEnemy.DeselectEnemy();
        }

        this.selectedEnemy = enemyReference;
        this.selectedEnemy.SelectEnemy();
        this.EnemySelected = true;
        DistanceOfEnemy = movementSystem.CalculateEnemyPathDistance(this.selectedEnemy.GetEnemyHexCoords(), this.hexGrid);

    }





    private void PrepareUnitForMovement(Unit unitReference)
    {

        if (this.selectedUnit != null)
        {
            ClearOldSelection();
        }

        this.selectedUnit = unitReference;
        this.selectedUnit.Select();
        //Debug.Log("Prepare Unit For Movement");
        movementSystem.ShowRange(this.selectedUnit, this.hexGrid);
    }

    private void ClearOldSelection()
    {
        previouslySelectedHex = null;
        this.selectedUnit.Deselect();
        movementSystem.HideRange(this.hexGrid);
        this.selectedUnit = null;

    }

    private void ClearOldEnemySelection()
    {
        previouslySelectedHex = null;
        this.selectedEnemy.DeselectEnemy();
        this.EnemySelected = false;
        this.selectedEnemy = null;

    }


    private void HandleTargetHexSelected(Hex selectedHex) // STARTS WALK HERE
    {
        if (previouslySelectedHex == null || previouslySelectedHex != selectedHex)
        {
            previouslySelectedHex = selectedHex;
            movementSystem.ShowPath(selectedHex.HexCoords, this.hexGrid);
        }
        else
        {
            movementSystem.MoveUnit(selectedUnit, this.hexGrid);
            selectedUnit.Walk();// Animation
            PlayersTurn = false;
            selectedUnit.MovementFinished += ResetTurn;
            ClearOldSelection();

            if (this.EnemySelected)
            {
                ClearOldEnemySelection();
            }

        }
    }

    private bool HandleSelectedHexIsUnitHex(Vector3Int hexPosition)
    {
        if (hexPosition == hexGrid.GetClosestHex(selectedUnit.transform.position))
        {
            selectedUnit.Deselect();
            ClearOldSelection();
            return true;
        }
        return false;
    }

    private bool HandleHexOutOfRange(Vector3Int hexPosition)
    {
        if (movementSystem.IsHexInRange(hexPosition) == false)
        {
            Debug.Log("Hex Out of range!");
            return true;
        }
        return false;
    }

    private bool HandleEnemyOutOfRange(Vector3Int hexPosition)
    {
        if (movementSystem.IsHexInRange(hexPosition) == false)
        {
            Debug.Log("Enemy Out of range!");
            return true;
        }
        return false;
    }




    private void ResetTurn(Unit selectedUnit)
    {
        selectedUnit.MovementFinished -= ResetTurn;
        PlayersTurn = true;
    }
}