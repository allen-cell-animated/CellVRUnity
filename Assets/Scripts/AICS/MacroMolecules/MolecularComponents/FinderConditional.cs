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
		public BindingSite lastBindingSiteFound;

		MoleculeDetector detector;
		protected List<BindingSite> validBindingSites = new List<BindingSite>();

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
			lastBindingSiteFound = Find();
			return lastBindingSiteFound != null;
		}

		public virtual BindingSite Find ()
		{
			validBindingSites.Clear();
			foreach (Molecule m in potentialMolecules)
			{
				validBindingSites.AddRange( m.GetOpenBindingSites( bindingCriteria ) );
			}

			if (validBindingSites.Count > 0)
			{
				return PickFromValidBindingSites();
			}
			return null;
		}

		protected virtual BindingSite PickFromValidBindingSites ()
		{
			return GetRandomBindingSite();
		}

		protected BindingSite GetRandomBindingSite ()
		{
			return validBindingSites[validBindingSites.GetRandomIndex()];
		}
	}
}