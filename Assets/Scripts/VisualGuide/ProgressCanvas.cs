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

    public void UpdateTime (float startTime)
    {
        float timeSeconds = Time.time - startTime;
        float timeMinutes = Mathf.Floor( timeSeconds / 60f );
        timeSeconds = Mathf.Round( timeSeconds - 60f * timeMinutes);

        string timeSecondsStr = timeSeconds.ToString();
        while (timeSecondsStr.Length < 2)
        {
            timeSecondsStr = "0" + timeSecondsStr;
        }

        string timeMinutesStr = timeMinutes.ToString();
        while (timeMinutesStr.Length < 2)
        {
            timeMinutesStr = "0" + timeMinutesStr;
        }

        UIManager.Instance.progressCanvas.time.text = timeMinutesStr + ":" + timeSecondsStr;
    }
}
