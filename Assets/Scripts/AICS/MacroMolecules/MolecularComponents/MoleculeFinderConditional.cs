using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class MoleculeFinderConditional : Conditional
	{
		public MoleculeType typeToFind;
		public float searchRadius = 15f;
		public bool onlyFindIfColliding = true;
		public IBind lastBinderFound;

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

		void Start ()
		{
			CreateDetector();
		}

		void CreateDetector ()
		{
			detector = (Instantiate( Resources.Load( "Prefabs/MoleculeDetector" ), transform ) as GameObject).GetComponent<MoleculeDetector>().Setup( typeToFind, searchRadius );
		}

		protected override bool DoCheck ()
		{
			lastBinderFound = Find();
			return lastBinderFound != null;
		}

		public IBind Find ()
		{
			validBinders.Clear();
			IBind otherBinder = null;
			foreach (Molecule m in potentialMolecules)
			{
				if (!molecule.IsBoundToOther( m ))
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