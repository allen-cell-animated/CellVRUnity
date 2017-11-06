using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Cell;

public class CellViveHead : MonoBehaviour
{
    public SteamVR_LoadLevel levelLoader;
    public List<CellViveController> controllers = new List<CellViveController>();

    bool canSwitchScene
    {
        get
        {
            return controllers.Find( c => c.scaling ) == null;
        }
    }

    void OnTriggerEnter (Collider other)
    {
        if (canSwitchScene)
        {
            Cell cell = other.GetComponent<Cell>();
            if (cell != null)
            {
                Debug.Log("entered cell");
                levelLoader.Trigger();
            }
        }
    }
}
