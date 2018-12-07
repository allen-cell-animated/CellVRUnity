using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoCanvas : MonoBehaviour 
{
    public Text title;
    public Image image;
    public Text text;

    public void SetContent (StructureData _data)
    {
        title.text = _data.structureName;
        image.sprite = _data.infoImage;
        text.text = _data.description;
    }
}
