using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CellStructure : MonoBehaviour 
{
    public string structureName;
    public Color color;
    [HideInInspector] public StructureData data;

    VRTK_InteractableObject _interactableObject;
    VRTK_InteractableObject interactableObject
    {
        get
        {
            if (_interactableObject == null)
            {
                _interactableObject = GetComponent<VRTK_InteractableObject>();
            }
            return _interactableObject;
        }
    }

    void OnEnable ()
    {
        if (interactableObject != null)
        {
            interactableObject.InteractableObjectTouched += OnHoverEnter;
            interactableObject.InteractableObjectUntouched += OnHoverExit;
        }
    }

    void OnDisable ()
    {
        if (interactableObject != null)
        {
            interactableObject.InteractableObjectTouched -= OnHoverEnter;
            interactableObject.InteractableObjectUntouched -= OnHoverExit;
        }
    }

    void OnHoverEnter (object sender, InteractableObjectEventArgs e)
    {
        VisualGuideManager.Instance.OnHoverStructureEnter( this );
    }

    void OnHoverExit (object sender, InteractableObjectEventArgs e)
    {
        VisualGuideManager.Instance.OnHoverStructureExit();
    }

    void Start ()
    {
        GrayOut( false );
        SetData( VisualGuideManager.Instance.data );
    }

    public void SetData (VisualGuideData _data)
    {
        data = _data.structureData.Find( s => s.structureName == structureName );
        if (_data == null)
        {
            Debug.LogWarning( "Couldn't load structure data for " + structureName );
        }
    }

    Material _material;
    Material material
    {
        get
        {
            if (_material == null)
            {
                _material = GetComponent<MeshRenderer>().material;
            }
            return _material;
        }
    }

    public void GrayOut (bool _gray)
    {
        if (!_gray)
        {
            material.SetColor( "_Color", color );
        }
        else
        {
            material.SetColor( "_Color", Color.white );
        }
    }
}
