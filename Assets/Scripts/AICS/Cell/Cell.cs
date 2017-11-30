using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Cell
{
    public class Cell : MonoBehaviour
    {
		public GameObject surface;
		public GameObject volume;
		public GameObject outline;

		public void SetRepresentation (string type)
		{
			surface.SetActive( type == "surface" );
			volume.SetActive( type == "volume" );
		}

		public void SetOutline (bool show)
		{
			outline.SetActive( show );
		}
    }
}