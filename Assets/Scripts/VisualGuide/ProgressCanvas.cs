using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressCanvas : MonoBehaviour 
{
    public Text time;
    public GameObject completeER;
    public GameObject completeGolgi;
    public GameObject completeMTs;
    public GameObject completeMitos;
    public Text structureLabel;

    public void SetStructureLabel (string structureName = "")
    {
        structureLabel.text = structureName;
    }

    public void SetComplete (string structureName, bool complete)
    {
        switch (structureName)
        {
            case "Endoplasmic Reticulum":
                completeER.SetActive( complete );
                return;

            case "Golgi Apparatus":
                completeGolgi.SetActive( complete );
                return;

            case "Microtubules":
                completeMTs.SetActive( complete );
                return;

            case "Mitochondria":
                completeMitos.SetActive( complete );
                return;
        }
    }
}
