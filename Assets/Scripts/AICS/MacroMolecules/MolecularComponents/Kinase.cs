using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class Kinase : MolecularComponent
	{
		public GameObject ATP;
		public GameObject ADP;
		public GameObject Pi;

		public void BindATP ()
		{
			SetDisplay( true, false, false );
		}

		public void BindADP ()
		{
			SetDisplay( false, true, false );
		}

		public void Hydrolyze ()
		{
			SetDisplay( false, true, true );
		}

		public void ReleaseATP ()
		{
			SetDisplay( false, false, false );
		}

		public void ReleaseADP ()
		{
			SetDisplay( false, false, false );
		}

		public void ReleasePi ()
		{
			SetDisplay( false, ADP.activeSelf, false );
		}

		void SetDisplay (bool showATP, bool showADP, bool showPi)
		{
			ATP.SetActive( showATP );
			ADP.SetActive( showADP );
			Pi.SetActive( showPi );
		}
	}
}