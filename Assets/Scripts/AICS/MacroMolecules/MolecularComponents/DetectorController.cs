using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class DetectorController : MolecularComponent
	{
		float originalRadius = -1f;

		public void SetDetectorToMoleculeRadius ()
		{
			if (originalRadius < 0)
			{
				originalRadius = molecule.detector.radius;
			}
			molecule.detector.SetRadius( molecule.radius );
		}

		public void ResetDetectorRadius ()
		{
			molecule.detector.SetRadius( originalRadius );
		}
	}
}