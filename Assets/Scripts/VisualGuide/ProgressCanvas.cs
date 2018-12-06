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

    public void AnimateTitle ()
    {
        titleAnimator.SetTrigger( "animate" );
    }

    public void SetSelected (string structureName, bool selected)
    {
        switch (structureName)
        {
            case "Endoplasmic Reticulum (ER)":
                selectedER.SetActive( true );
                selectedGolgi.SetActive( false );
                selectedMTs.SetActive( false );
                selectedMitos.SetActive( false );
                return;

            case "Golgi Apparatus":
                selectedER.SetActive( false );
                selectedGolgi.SetActive( true );
                selectedMTs.SetActive( false );
                selectedMitos.SetActive( false );
                return;

            case "Microtubules":
                selectedER.SetActive( false );
                selectedGolgi.SetActive( false );
                selectedMTs.SetActive( true );
                selectedMitos.SetActive( false );
                return;

            case "Mitochondria":
                selectedER.SetActive( false );
                selectedGolgi.SetActive( false );
                selectedMTs.SetActive( false );
                selectedMitos.SetActive( true );
                return;
        }
    }

    public void SelectStructureInUI (string structureName)
    {
        VisualGuideManager.Instance.interphaseCell.SelectStructure( structureName );
    }

    public void GoBack ()
    {
        VisualGuideManager.Instance.ReturnToLobby();
    }
}
