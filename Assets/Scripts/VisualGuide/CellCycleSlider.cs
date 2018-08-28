using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellCycleSlider : MonoBehaviour 
{
	public Slider slider;
    public GameObject[] cells;

    float[] bins = {0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f};
    GameObject currentCell;

    void Start ()
    {
        currentCell = cells[0];
    }

	public void SetValue (float _sliderValue)
    {
        DiscretizeSliderPosition(_sliderValue);
    }

    void DiscretizeSliderPosition (float _sliderValue)
    {
        for (int i = 0; i < bins.Length - 1; i++)
        {
            if (_sliderValue > bins[i] && _sliderValue <= bins[i+1])
            {
                slider.value = (bins[i] + bins[i+1]) / 2f;
                DisplayCell( i + 1 );
                return;
            }
        }
        DisplayCell( 0 );
    }

    void DisplayCell (int i)
    {
        if (currentCell != cells[i])
        {
            VisualGuideManager.Instance.ExitIsolationMode();
            if (currentCell != null)
            {
                currentCell.SetActive( false );
            }
            cells[i].SetActive( true );
            currentCell = cells[i];
        }
    }
}
