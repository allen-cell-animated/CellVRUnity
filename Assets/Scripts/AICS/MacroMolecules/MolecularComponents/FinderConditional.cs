using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class FinderConditional : Conditional
	{
		public MoleculeType typeToFind;
		public float searchRadius = 15f;
		public bool onlyFindIfColliding = true;
		public MoleculeBinder lastBinderFound;

		MoleculeDetector detector;
		protected List<MoleculeBinder> validBinders = new List<MoleculeBinder>();

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

		public virtual MoleculeBinder Find ()
		{
			validBinders.Clear();
			MoleculeBinder otherBinder = null;
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

		protected virtual MoleculeBinder GetValidBinder (Molecule other)
		{
			return other.GetOpenBinder( molecule.type ) as MoleculeBinder;
		}

		protected virtual MoleculeBinder PickFromValidBinders ()
		{
			return GetRandomBinder();
		}

		protected MoleculeBinder GetRandomBinder ()
		{
			return validBinders[validBinders.GetRandomIndex()];
		}
	}
}