using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class FinderConditional : Conditional
	{
		public BindingCriteria bindingCriteria;
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
			detector = (Instantiate( Resources.Load( "Prefabs/MoleculeDetector" ), transform ) as GameObject)
				.GetComponent<MoleculeDetector>().Setup( bindingCriteria.typeToBind, searchRadius );
		}

		protected override bool DoCheck ()
		{
			lastBinderFound = Find();
			return lastBinderFound != null;
		}

		public virtual MoleculeBinder Find ()
		{
			if (detector.gameObject.activeSelf)
			{
				validBinders.Clear();
				foreach (Molecule m in potentialMolecules)
				{
					if (m != molecule && !molecule.IsBoundToMolecule( m ))
					{
						validBinders.AddRange( m.GetOpenBinders( bindingCriteria ) );
					}
				}
				if (validBinders.Count > 0)
				{
					return PickFromValidBinders();
				}
			}
			return null;
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