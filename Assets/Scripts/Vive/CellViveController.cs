using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AICS.Cell;

public enum CellViveControllerState
{
    Idle,
    HoldingCell,
    Scaling
}

public class CellViveController : ViveController
{
    public CellViveControllerState state;
    public CellViveController otherController;
    public Cell hoveredCell;

    public Cell draggedCell
    {
        get
        {
            return GetComponentInChildren<Cell>();
        }
    }

    float startControllerDistance;
    Vector3 startCellScale;
    Vector3 minScale = new Vector3( 0.2f, 0.2f, 0.08f );
    Vector3 maxScale = new Vector3( 2.5f, 2.5f, 1f );

    void OnTriggerEnter (Collider other)
    {
        Cell cell = other.GetComponent<Cell>();
        if (cell != null)
        {
            hoveredCell = cell;
        }
    }

    void OnTriggerExit (Collider other)
    {
        Cell cell = other.GetComponent<Cell>();
        if (cell != null && hoveredCell == cell)
        {
            hoveredCell = null;
        }
    }

    public override void OnTriggerPull () 
	{
        if (state == CellViveControllerState.Idle)
        {
            if (otherController.state == CellViveControllerState.HoldingCell)
            {
                StartScaling();
            }
            else
            {
                PickupCell();
            }
        }
    }

    public override void OnTriggerHold ()
    {
        if (state == CellViveControllerState.Scaling && otherController.state == CellViveControllerState.HoldingCell)
        {
            UpdateScale();
        }
    }

    public override void OnTriggerRelease()
    {
        if (state == CellViveControllerState.Scaling)
        {
            StopScaling();
        }
        else if (state == CellViveControllerState.HoldingCell)
        {
            ReleaseCell();
        }
    }

    public void SwitchToPrimary()
    {
        if (state == CellViveControllerState.Scaling)
        {
            StopScaling();
            PickupCell();
        }
    }

    Vector3 ClampScale (Vector3 scale)
    {
        if (scale.magnitude > maxScale.magnitude)
        {
            return maxScale;
        }
        else if (scale.magnitude < minScale.magnitude)
        {
            return minScale;
        }
        else
        {
            return scale;
        }
    }

    void PickupCell ()
    {
        if (hoveredCell != null)
        {
            hoveredCell.transform.SetParent( transform );
            state = CellViveControllerState.HoldingCell;
        }
    }

    void ReleaseCell ()
    {
        Cell cell = draggedCell;
        if (cell != null)
        {
            cell.transform.SetParent( null );
        }
        state = CellViveControllerState.Idle;
        otherController.SwitchToPrimary();
    }

    void StartScaling ()
    {
        Cell cell = otherController.draggedCell;
        if (cell != null)
        {
            startControllerDistance = Vector3.Distance( transform.position, otherController.transform.position );
            startCellScale = cell.transform.localScale;
            state = CellViveControllerState.Scaling;
        }
    }

    void UpdateScale ()
    {
        Cell cell = otherController.draggedCell;
        if (cell != null)
        {
            float d = Vector3.Distance( transform.position, otherController.transform.position );
            cell.transform.localScale = ClampScale( (d / startControllerDistance) * startCellScale );
        }
    }

    void StopScaling ()
    {
        state = CellViveControllerState.Idle;
    }
}
