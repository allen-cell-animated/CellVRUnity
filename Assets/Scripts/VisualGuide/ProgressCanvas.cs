using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressCanvas : MonoBehaviour 
{
    public Text time;
    public GameObject selectedER;
    public GameObject selectedGolgi;
    public GameObject selectedMTs;
    public GameObject selectedMitos;
    public Text titleLabel;

    Animator _titleAnimator;
    public Animator titleAnimator
    {
        get
        {
            if (_titleAnimator == null)
            {
                _titleAnimator = titleLabel.gameObject.GetComponent<Animator>();
            }
            return _titleAnimator;
        }
    }

    public void SetTitle (string structureName = "Choose a structure:")
    {
        titleLabel.text = structureName;
        if (structureName != "Choose a structure:")
        {
            titleAnimator.SetTrigger( "animate" );
        }
    }

    public void SetSelected (string structureName, bool selected)
    {
        switch (structureName)
        {
            case "Endoplasmic Reticulum":
                selectedER.SetActive( selected );
                return;

            case "Golgi Apparatus":
                selectedGolgi.SetActive( selected );
                return;

            case "Microtubules":
                selectedMTs.SetActive( selected );
                return;

            case "Mitochondria":
                selectedMitos.SetActive( selected );
                return;
        }
    }

    public void SelectStructureInUI (string structureName)
    {
        VisualGuideManager.Instance.interphaseCell.SelectStructure( structureName );
    }
}
