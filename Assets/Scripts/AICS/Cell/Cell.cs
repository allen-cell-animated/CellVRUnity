using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Cell
{
    public class Cell : MonoBehaviour
    {
		public GameObject surface;
		public GameObject volume;

		public void SetRepresentation (string type)
		{
			surface.SetActive( type == "surface" );
			volume.SetActive( type == "volume" );
		}
    }
}