using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AICS.Cell;

public class CellViveController : ViveController
{
    public Cell currentCell;

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
        if (currentCell != null)
        {
            currentCell.transform.SetParent( transform );
        }
	}

	public override void OnTriggerRelease () 
	{
        if (currentCell != null)
        {
            currentCell.transform.SetParent( null );
        }
    }
}
