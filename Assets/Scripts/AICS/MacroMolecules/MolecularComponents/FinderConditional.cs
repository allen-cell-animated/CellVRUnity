using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class FinderConditional : Conditional, IFind
	{
		public BindingCriteria _criteriaToFind;
		public BindingCriteria criteriaToFind 
		{
			get
			{
				return _criteriaToFind;
			}
		}

		public float _searchRadius = 15f;
		public float searchRadius 
		{ 
			get
			{
				return _searchRadius;
			}
		}

		public bool onlyFindIfColliding = true;
		public MoleculeBinder lastBinderFound;

		protected List<MoleculeBinder> validBinders = new List<MoleculeBinder>();

		List<Molecule> potentialMolecules
		{
			get
			{
				return molecule.detector.GetNearbyMolecules( transform.position, onlyFindIfColliding ? molecule.radius : searchRadius );
			}
		}

		protected override bool DoCheck ()
		{
			lastBinderFound = Find();
			return lastBinderFound != null;
		}

		public virtual MoleculeBinder Find ()
		{
			if (molecule.detector.gameObject.activeSelf)
			{
				validBinders.Clear();
				foreach (Molecule m in potentialMolecules)
				{
					if (m != molecule && !molecule.IsBoundToMolecule( m ) && MoleculeIsValid( m ))
					{
						validBinders.AddRange( m.GetOpenBinders( criteriaToFind ) );
					}
				}
				if (validBinders.Count > 0)
				{
					return PickFromValidBinders();
				}
			}
			return null;
		}

		protected virtual bool MoleculeIsValid (Molecule _molecule)
		{
			return true;
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