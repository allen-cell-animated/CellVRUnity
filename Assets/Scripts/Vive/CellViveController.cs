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
	public Cell[] cells;
	public GameObject surfaceButtonLabel;
	public GameObject volumeButtonLabel;
	public GameObject surfaceButtonHoverLabel;
	public GameObject volumeButtonHoverLabel;

    public Cell draggedCell
    {
        get
        {
            return GetComponentInChildren<Cell>();
        }
    }

    float startControllerDistance;
    Vector3 startCellScale;
    Vector3 minScale = new Vector3( 0.2f, 0.2f, 0.2f );
    Vector3 maxScale = new Vector3( 2.5f, 2.5f, 2.5f );
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
		SetRepresentationButtons();
	}

	void UpdateButtonLabels ()
	{
		if (state == CellViveControllerState.Idle && otherController.state == CellViveControllerState.Idle)
		{
			ShowObject( grabButtonLabel, true );
			ShowObject( scaleButtonLabel, false );
			ShowObject( labelLine, true );
		}
		else if (state == CellViveControllerState.Idle && otherController.state == CellViveControllerState.HoldingCell)
		{
			ShowObject( grabButtonLabel, false );
			ShowObject( scaleButtonLabel, true );
			ShowObject( labelLine, true );
		}
		else
		{
			ShowObject( grabButtonLabel, false );
			ShowObject( scaleButtonLabel, false );
			ShowObject( labelLine, false );
		}
	}

	void ShowObject (GameObject label, bool show)
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

	public override void OnDPadEnter () 
	{
        hovering = true;
	}

	public override void OnDPadExit () 
	{
        hovering = false;
	}

	public override void OnDPadPressed () 
	{
		SwitchRepresentations();
	}

    public bool canSwitchReps = true;
	bool volumeOn = false;
	bool hovering = false;

	void SwitchRepresentations ()
	{
        if (canSwitchReps)
        {
            volumeOn = !volumeOn;
            foreach (Cell cell in cells)
            {
                cell.SetRepresentation(volumeOn ? "volume" : "surface");
            }
            surfaceButtonLabel.SetActive(!volumeOn);
            volumeButtonLabel.SetActive(volumeOn);
        }
	}

	void SetRepresentationButtons ()
	{
        if (canSwitchReps)
        {
            surfaceButtonLabel.SetActive(volumeOn && !hovering);
            surfaceButtonHoverLabel.SetActive(volumeOn && hovering);
            volumeButtonLabel.SetActive(!volumeOn && !hovering);
            volumeButtonHoverLabel.SetActive(!volumeOn && hovering);
        }
	}
}
