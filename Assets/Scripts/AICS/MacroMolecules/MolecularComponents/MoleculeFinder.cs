using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class MoleculeFinder : MolecularComponent 
	{
		public MoleculeType typeToFind;
		public float searchRadius = 15f;
		public bool onlyFindIfColliding = true;

		MoleculeDetector detector;
		protected List<IBind> validBinders = new List<IBind>();

		List<Molecule> potentialMolecules
		{
			get
			{
				if (onlyFindIfColliding)
				{
					return detector.GetCollidingMolecules( transform.position, molecule.radius );
				}
				else
				{
					return detector.nearbyMolecules;
				}
			}
		}

		void Awake ()
		{
			CreateDetector();
		}

		void CreateDetector ()
		{
			detector = (Instantiate( Resources.Load( "Prefabs/MoleculeDetector" ), transform ) as GameObject).GetComponent<MoleculeDetector>().Setup( typeToFind, searchRadius );
		}

		public IBind Find (List<Molecule> excludedMolecules = null)
		{
			validBinders.Clear();
			IBind otherBinder = null;
			foreach (Molecule m in potentialMolecules)
			{
				if ((excludedMolecules == null || !excludedMolecules.Contains( m )))
				{
					otherBinder = GetValidBinder( m );
					if (otherBinder != null)
					{
						validBinders.Add( otherBinder );
					}
				}
			}
			if (validBinders.Count > 0)
			{
				return PickFromValidBinders();
			}
			return null;
		}

		protected virtual IBind GetValidBinder (Molecule other)
		{
			return other.GetOpenBinder( molecule.type );
		}

		protected virtual IBind PickFromValidBinders ()
		{
			return validBinders[validBinders.GetRandomIndex()];
		}
	}
}
