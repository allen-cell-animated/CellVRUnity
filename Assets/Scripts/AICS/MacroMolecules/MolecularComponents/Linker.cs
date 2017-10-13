using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class Linker : MolecularComponent, ISimulate
	{
		public Transform[] attachmentPoints;

		public void DoSimulationStep ()
		{
			PositionBetweenAttachments();
		}

		void PositionBetweenAttachments () 
		{
			int n = 0;
			Vector3 sumOfPositions = Vector3.zero;
			foreach (Transform point in attachmentPoints)
			{
				if (point != null)
				{
					sumOfPositions += point.position;
					n++;
				}
			}
			molecule.SetPosition( sumOfPositions / n );
		}
	}
}