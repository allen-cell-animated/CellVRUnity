using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AICS.Cell;

public enum VisualGuideControllerState
{
    Idle,
    HoldingTrigger
}

public class VisualGuideController : ViveController
{
	public VisualGuideControllerState state;
    public VisualGuideController otherController;
    public GameObject cell;
    public CellStructure hoveredStructure;
	public LineRenderer scaleLine;
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
    Vector3 minScale = new Vector3( 0.2f, 0.2f, 0.2f );
    Vector3 maxScale = new Vector3( 10f, 10f, 10f );
	Vector3[] linePoints = new Vector3[2];

    void OnTriggerEnter (Collider other)
    {
        CellStructure structure = other.GetComponentInParent<CellStructure>();
        if (structure != null)
        {
            SetHoveredStructureOutline( false );
            hoveredStructure = structure;
            SetHoveredStructureOutline( true );
        }
    }

    void OnTriggerExit (Collider other)
    {
        CellStructure structure = other.GetComponentInParent<CellStructure>();
        if (structure != null && hoveredStructure == structure)
        {
            SetHoveredStructureOutline( false );
            hoveredStructure = null;
        }
    }

    void SetHoveredStructureOutline (bool enabled)
    {
        if (hoveredStructure != null)
        {
            hoveredStructure.SetOutline( enabled );
        }
    }

    public override void OnTriggerPull () 
	{
        if (state == VisualGuideControllerState.Idle)
        {
            state = VisualGuideControllerState.HoldingTrigger;
            if (otherController.state == VisualGuideControllerState.HoldingTrigger)
            {
                StartScaling();
            }
        }
    }

    public override void OnTriggerHold ()
    {
        if (state == VisualGuideControllerState.HoldingTrigger && otherController.state == VisualGuideControllerState.HoldingTrigger)
        {
            UpdateScale();
        }
    }

    public override void OnTriggerRelease()
    {
        if (state == VisualGuideControllerState.HoldingTrigger)
        {
            state = VisualGuideControllerState.Idle;
            if (otherController.state == VisualGuideControllerState.Idle)
            {
                StopScaling();
            }
        }
    }

    void StartScaling ()
    {
        if (cell != null)
        {
            startControllerDistance = Vector3.Distance( transform.position, otherController.transform.position );
            startCellScale = cell.transform.localScale;
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
        if (cell != null)
        {
            float d = Vector3.Distance( transform.position, otherController.transform.position );
            cell.transform.localScale = ClampScale( (d / startControllerDistance) * startCellScale );
			SetLine();
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

    void StopScaling ()
    {
		scaleLine.gameObject.SetActive( false );
	}

	protected override void DoUpdate ()
	{
		UpdateButtonLabels();
	}

	void UpdateButtonLabels ()
	{
		if (state == VisualGuideControllerState.Idle)
		{
            ShowObject( scaleButtonLabel, true );
			ShowObject( labelLine, true );
		}
		else
		{
			ShowObject( scaleButtonLabel, false );
			ShowObject( labelLine, false );
		}
	}

	void ShowObject (GameObject obj, bool show)
	{
        if (show && !obj.activeSelf)
		{
            obj.SetActive( true );
		}
        else if (!show && obj.activeSelf)
		{
            obj.SetActive( false );
		}
	}
}
