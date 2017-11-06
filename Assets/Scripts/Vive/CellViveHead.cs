using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Cell;

public class CellViveHead : MonoBehaviour
{
    public bool canSwitch = true;
    public SteamVR_LoadLevel levelLoader;
    public List<CellViveController> controllers = new List<CellViveController>();

    bool canSwitchScene
    {
        get
        {
            return canSwitch && controllers.Find( c => c.state == CellViveControllerState.Scaling ) == null 
                && controllers.Find( c => c.state == CellViveControllerState.HoldingCell ) != null;
        }
    }

    void OnTriggerEnter (Collider other)
    {
        if (canSwitchScene)
        {
            Cell cell = other.GetComponent<Cell>();
            if (cell != null)
            {
                levelLoader.Trigger();
            }
        }
    }
}
