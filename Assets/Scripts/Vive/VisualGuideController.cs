using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AICS.Cell;

public enum VisualGuideControllerState
{
    Idle,
    FirstTriggerHold,
    SecondTriggerHold
}

public class VisualGuideController : ViveController
{
    public bool canSelect;
	public VisualGuideControllerState state;
    public VisualGuideController otherController;
    public CellStructure hoveredStructure;
	public LineRenderer scaleLine;
    public GameObject scaleButtonLabel;
    public GameObject selectButtonLabel;
    public GameObject backButtonLabel;
	public GameObject labelLine;
    public Transform cursor;
    public Material uiOverlayMaterial;
    public LayerMask selectableLayersDefault;
    public LayerMask selectableLayersIsolation;

	Vector3[] linePoints = new Vector3[2];
    GameObject[] buttonLabels = new GameObject[3];

    SteamVR_LaserPointer _laserPointer;
    SteamVR_LaserPointer laserPointer
    {
        get
        {
            if (_laserPointer == null)
            {
                _laserPointer = GetComponent<SteamVR_LaserPointer>();
            }
            return _laserPointer;
        }
    }

    void OnEnable ()
    {
        if (canSelect)
        {
            laserPointer.PointerIn += OnPointerEnter;
            laserPointer.PointerOut += OnPointerExit;
        }
    }

    void OnDisable ()
    {
        if (canSelect)
        {
            laserPointer.PointerIn -= OnPointerEnter;
            laserPointer.PointerOut -= OnPointerExit;
        }
    }

    void Start ()
    {
        buttonLabels[0] = scaleButtonLabel;
        buttonLabels[1] = selectButtonLabel;
        buttonLabels[2] = backButtonLabel;
    }

    void OnPointerEnter (object sender, PointerEventArgs args)
    {
        if (args.target != null)
        {
            if (!VisualGuideManager.Instance.inIsolationMode)
            {
                CellStructure structure = args.target.GetComponentInParent<CellStructure>();
                if (structure != null)
                {
                    SetHoveredStructure( false );
                    hoveredStructure = structure;
                    SetHoveredStructure( true );
                }
            }
            else 
            {
                Button button = args.target.GetComponent<Button>();
                if (button != null)
                {
                    Debug.Log("should select button");
                    button.Select();
                }
            }
        }
    }

    void OnPointerExit (object sender, PointerEventArgs args)
    {
        if (args.target != null)
        {
            CellStructure structure = args.target.GetComponentInParent<CellStructure>();
            if (structure != null && hoveredStructure == structure)
            {
                SetHoveredStructure( false );
                hoveredStructure = null;
            }
            else 
            {
                Button button = args.target.GetComponent<Button>();
                if (button != null)
                {
                    EventSystem.current.SetSelectedGameObject( null );
                }
            }
        }
    }

    void SetHoveredStructure (bool _enabled)
    {
        if (hoveredStructure != null)
        {
            hoveredStructure.SetOutline( _enabled );
            if (_enabled)
            {
                VisualGuideManager.Instance.LabelStructure( hoveredStructure );
            }
            else
            {
                VisualGuideManager.Instance.HideLabel();
            }
        }
    }

    public override void OnTriggerPull () 
	{
        if (state == VisualGuideControllerState.Idle)
        {
            if (otherController.state != VisualGuideControllerState.Idle)
            {
                otherController.state = VisualGuideControllerState.FirstTriggerHold;
                state = VisualGuideControllerState.SecondTriggerHold;
                StartTranslating();
            }
            else
            {
                state = VisualGuideControllerState.FirstTriggerHold;
            }
        }
    }

    public override void OnTriggerHold ()
    {
        if (state == VisualGuideControllerState.SecondTriggerHold)
        {
            UpdateTranslation();
        }
    }
    
    public override void OnTriggerRelease()
    {
        if (state == VisualGuideControllerState.FirstTriggerHold)
        {
            if (otherController.state == VisualGuideControllerState.SecondTriggerHold)
            {
                otherController.state = VisualGuideControllerState.FirstTriggerHold;
                otherController.StopTranslating();
            }
            else 
            {
                if (!VisualGuideManager.Instance.scaling && !VisualGuideManager.Instance.rotating)
                {
                    ToggleIsolationMode();
                }
                //VisualGuideManager.Instance.StopScaling();
            }
        }
        else if (state == VisualGuideControllerState.SecondTriggerHold)
        {
            StopTranslating();
        }
        state = VisualGuideControllerState.Idle;
    }

    void StartTranslating ()
    {
        VisualGuideManager.Instance.HideLabel();
        VisualGuideManager.Instance.StartScaling( transform.position, otherController.transform.position );
        VisualGuideManager.Instance.StartRotating( transform.position, otherController.transform.position );
        ToggleLaser( false );
        SetLine( true );
    }

    public void UpdateTranslation ()
    {
        VisualGuideManager.Instance.UpdateScale( transform.position, otherController.transform.position );
        VisualGuideManager.Instance.UpdateRotation( transform.position, otherController.transform.position );
        SetLine( true );
    }

    void StopTranslating ()
    {
        VisualGuideManager.Instance.StopScaling();
        VisualGuideManager.Instance.StopRotating();
        ToggleLaser( true );
        SetLine( false );
    }

	void SetLine (bool active)
	{
        if (active)
        {
            if (!scaleLine.gameObject.activeSelf)
            {
                scaleLine.gameObject.SetActive( true );
            }
            linePoints[0] = transform.position;
            linePoints[1] = otherController.transform.position;
            scaleLine.SetPositions( linePoints );
        }
        else
        {
            scaleLine.gameObject.SetActive( false );
        }
	}

    void ToggleIsolationMode ()
    {
        if (!VisualGuideManager.Instance.inIsolationMode)
        {
            if (hoveredStructure != null)
            {
                laserPointer.selectableLayers = selectableLayersIsolation;
                VisualGuideManager.Instance.IsolateStructure( hoveredStructure );
            }
        }
        else if (canSelect)
        {
            laserPointer.selectableLayers = selectableLayersDefault;
            VisualGuideManager.Instance.ExitIsolationMode();
        }
    }

    public void ToggleLaser (bool _on)
    {
        if (canSelect)
        {
            laserPointer.pointer.SetActive( _on );
        }
        else if (otherController.canSelect)
        {
            otherController.laserPointer.pointer.SetActive( _on );
        }
    }

    protected override void DoUpdate()
    {
        UpdateButtonLabels();
        if (canSelect)
        {
            Debug.Log(EventSystem.current.currentSelectedGameObject == null ? "null" : EventSystem.current.currentSelectedGameObject.name);
        }
	}

	void UpdateButtonLabels ()
	{
		if (state == VisualGuideControllerState.Idle)
		{
            if (otherController.state == VisualGuideControllerState.Idle)
            {
                if (!VisualGuideManager.Instance.inIsolationMode)
                {
                    if (hoveredStructure != null)
                    {
                        SetButtonLabel( selectButtonLabel );
                    }
                    else
                    {
                        SetButtonLabel( scaleButtonLabel );
                    }
                }
                else
                {
                    if (canSelect)
                    {
                        SetButtonLabel( backButtonLabel );
                    }
                    else
                    {
                        SetButtonLabel( scaleButtonLabel );
                    }
                }
            }
            else
            {
                SetButtonLabel( scaleButtonLabel );
            }
            ShowObject( labelLine, true );
		}
		else
		{
            SetButtonLabel( null );
            ShowObject( labelLine, false );
		}
	}

    void SetButtonLabel (GameObject _buttonLabel)
	{
        foreach (GameObject buttonLabel in buttonLabels)
        {
            if (buttonLabel != _buttonLabel)
            {
                ShowObject( buttonLabel, false );
            }
            else
            {
                ShowObject( buttonLabel, true );
            }
        }
	}

    void ShowObject (GameObject obj, bool show)
    {
        if (obj != null)
        {
            if (show && !obj.activeSelf)
            {
                obj.SetActive( true );
            }
            else if (!show && obj.activeSelf)
            {
                obj.SetActive( false );
            }
        }
    }
}
