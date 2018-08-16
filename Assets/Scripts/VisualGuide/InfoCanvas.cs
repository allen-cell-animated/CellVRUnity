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
	
    public void SetContent (StructureData _data)
    {
        title.text = _data.structureName;
        image.sprite = _data.infoImage;
        text.text = _data.description;

        SetPosition();
    }

    void Start ()
    {
        SetPosition();
    }

    void SetPosition ()
    {
        Vector3 position = Camera.main.transform.position + Camera.main.transform.TransformPoint( offsetFromCamera );
        transform.position = new Vector3( position.x, Camera.main.transform.position.y, position.z );
        transform.LookAt( transform.position + (transform.position - Camera.main.transform.position), Vector3.up );
        transform.rotation = Quaternion.Euler( 0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z );
    }
}
