using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Cell;

public class CellViveHead : MonoBehaviour
{
    public SteamVR_LoadLevel levelLoader;

    void OnTriggerEnter (Collider other)
    {
        Cell cell = other.GetComponent<Cell>();
        if (cell != null)
        {
            Debug.Log("entered cell");
            levelLoader.Trigger();
        }
    }
}
