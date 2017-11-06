using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AICS.Cell;

public class CellViveController : ViveController
{
    public CellViveController otherController;
    public Cell currentCell;
    public bool holdingCell = false;
    public bool scaling = false;

    float startControllerDistance;
    Vector3 startCellScale;
    Vector3 minScale = new Vector3( 0.2f, 0.2f, 0.08f );
    Vector3 maxScale = new Vector3( 2.5f, 2.5f, 1f );

    void OnTriggerEnter (Collider other)
    {
        Cell cell = other.GetComponent<Cell>();
        if (cell != null)
        {
            currentCell = cell;
        }
    }

    void OnTriggerExit (Collider other)
    {
        Cell cell = other.GetComponent<Cell>();
        if (cell != null && currentCell == cell)
        {
            currentCell = null;
        }
    }

    public override void OnTriggerPull () 
	{
        if (otherController.currentCell != null)
        {
            startControllerDistance = Vector3.Distance( transform.position, otherController.transform.position );
            startCellScale = otherController.currentCell.transform.localScale;
            scaling = true;
        }
        else if (currentCell != null)
        {
            currentCell.transform.SetParent( transform );
            holdingCell = true;
        }
    }

    public override void OnTriggerHold ()
    {
        if (scaling)
        {
            float d = Vector3.Distance(transform.position, otherController.transform.position);
            otherController.currentCell.transform.localScale = ClampScale( (d / startControllerDistance) * startCellScale );
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

    public override void OnTriggerRelease () 
	{
        if (scaling)
        {
            scaling = false;
        }
        else if (currentCell != null)
        {
            currentCell.transform.SetParent( null );
            holdingCell = false;
        }
    }
}
