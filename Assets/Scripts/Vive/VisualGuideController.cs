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

    float startControllerDistance;
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
                StartScaling();
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
            UpdateScale();
        }
    }
    
    public override void OnTriggerRelease()
    {
        if (state == VisualGuideControllerState.FirstTriggerHold)
        {
            if (otherController.state == VisualGuideControllerState.SecondTriggerHold)
            {
                otherController.state = VisualGuideControllerState.FirstTriggerHold;
                otherController.StopScaling();
            }
            else 
            {
                if (!VisualGuideManager.Instance.scaling)
                {
                    ToggleIsolationMode();
                }
                VisualGuideManager.Instance.StopScaling();
            }
        }
        else if (state == VisualGuideControllerState.SecondTriggerHold)
        {
            StopScaling();
        }
        state = VisualGuideControllerState.Idle;
    }

    void StartScaling ()
    {
        VisualGuideManager.Instance.StartScaling();

        startControllerDistance = Vector3.Distance( transform.position, otherController.transform.position );
        scaleLine.gameObject.SetActive( true );
        SetLine();
    }

    public void UpdateScale ()
    {
        VisualGuideManager.Instance.UpdateScale( Vector3.Distance( transform.position, otherController.transform.position ) / startControllerDistance );

        SetLine();
    }

	void SetLine ()
	{
		linePoints[0] = transform.position;
		linePoints[1] = otherController.transform.position;
		scaleLine.SetPositions( linePoints );
	}

    void StopScaling ()
    {
		scaleLine.gameObject.SetActive( false );
	}

    void ToggleIsolationMode ()
    {
        if (!VisualGuideManager.Instance.inIsolationMode)
        {
            if (hoveredStructure != null)
            {
                VisualGuideManager.Instance.IsolateStructure( hoveredStructure );
            }
        }
        else if (canSelect)
        {
            VisualGuideManager.Instance.ExitIsolationMode();
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
