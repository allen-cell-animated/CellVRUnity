using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoCanvas : MonoBehaviour 
{
    public Text title;
    public Image image;
    public Text text;
	
    public void SetContent (string _title, Sprite _image, string _copy)
    {
        title.text = _title;
        image.sprite = _image;
        text.text = _copy;

        SetPosition();
    }

    void SetPosition ()
    {
        //TODO, use Camera.main.transform.position
    }
}
