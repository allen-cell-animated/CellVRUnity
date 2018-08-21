using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoCanvas : MonoBehaviour 
{
    public Text title;
    public Image image;
    public Text text;

    Vector3 offsetFromCamera = new Vector3( -1f, 0, 1f );

    Transform _calculator;
    Transform calculator
    {
        get
        {
            if (_calculator == null)
            {
                _calculator = new GameObject( "Calculator" ).transform;
            }
            return _calculator;
        }
    }

    void Start ()
    {
        gameObject.SetActive( false );
    }

    public void SetContent (StructureData _data)
    {
        title.text = _data.structureName;
        image.sprite = _data.infoImage;
        text.text = _data.description;

        SetPosition();
    }

    void SetPosition ()
    {
        calculator.position = Camera.main.transform.position;
        calculator.rotation = Quaternion.Euler( 0, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z );

        Vector3 position = calculator.TransformPoint( offsetFromCamera );
        transform.position = new Vector3( position.x, calculator.position.y, position.z );

        transform.LookAt( transform.position + (transform.position - Camera.main.transform.position) );
        transform.rotation = Quaternion.Euler( 0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z );
    }
}
