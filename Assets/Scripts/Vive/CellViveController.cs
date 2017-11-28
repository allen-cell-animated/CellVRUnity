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
	public LineRenderer scaleLine;
	public GameObject grabButtonLabel;
	public GameObject scaleButtonLabel;
	public GameObject labelLine;

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
	Vector3[] linePoints = new Vector3[2];

    void OnTriggerEnter (Collider other)
    {
        Cell cell = other.GetComponentInParent<Cell>();
        if (cell != null)
        {
            hoveredCell = cell;
        }
    }

    void OnTriggerExit (Collider other)
    {
        Cell cell = other.GetComponentInParent<Cell>();
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
			scaleLine.gameObject.SetActive( true );
			SetLine();
        }
    }

	void SetLine ()
	{
		linePoints[0] = transform.position;
		linePoints[1] = otherController.transform.position;
		scaleLine.SetPositions( linePoints );
	}

    void UpdateScale ()
    {
        Cell cell = otherController.draggedCell;
        if (cell != null)
        {
            float d = Vector3.Distance( transform.position, otherController.transform.position );
            cell.transform.localScale = ClampScale( (d / startControllerDistance) * startCellScale );
			SetLine();
        }
    }

    void StopScaling ()
    {
        state = CellViveControllerState.Idle;
		scaleLine.gameObject.SetActive( false );
	}

	protected override void DoUpdate ()
	{
		UpdateButtonLabels();
	}

	void UpdateButtonLabels ()
	{
		if (state == CellViveControllerState.Idle && otherController.state == CellViveControllerState.Idle)
		{
			ShowLabel( grabButtonLabel, true );
			ShowLabel( scaleButtonLabel, false );
			ShowLabel( labelLine, true );
		}
		else if (state == CellViveControllerState.Idle && otherController.state == CellViveControllerState.HoldingCell)
		{
			ShowLabel( grabButtonLabel, false );
			ShowLabel( scaleButtonLabel, true );
			ShowLabel( labelLine, true );
		}
		else
		{
			ShowLabel( grabButtonLabel, false );
			ShowLabel( scaleButtonLabel, false );
			ShowLabel( labelLine, false );
		}
	}

	void ShowLabel (GameObject label, bool show)
	{
		if (show && !label.activeSelf)
		{
			label.SetActive( true );
		}
		else if (!show && label.activeSelf)
		{
			label.SetActive( false );
		}
	}
}
