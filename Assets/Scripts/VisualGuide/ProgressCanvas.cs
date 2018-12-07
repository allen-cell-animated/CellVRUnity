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
    public GameObject backLabel;
    public GameObject nextLabel;

    Animator _animator;
    public Animator animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = gameObject.GetComponent<Animator>();
            }
            return _animator;
        }
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

    public void SetButtonLabel (bool next)
    {
        backLabel.SetActive( !next );
        nextLabel.SetActive( next );
    }

    public void GoBack ()
    {
        VisualGuideManager.Instance.ReturnToLobby();
    }
}
