using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class MoleculeFinder : MolecularComponent 
	{
		public MoleculeType moleculeToFind;
		public float searchRadius = 15f;
		public bool onlyFindIfColliding = true;

		MoleculeDetector detector;
		protected List<Molecule> validMolecules = new List<Molecule>();

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
			detector = (Instantiate( Resources.Load( "Prefabs/MoleculeDetector" ), transform ) as GameObject).GetComponent<MoleculeDetector>().Setup( moleculeToFind, searchRadius );
		}

		public Molecule Find (List<Molecule> excludedMolecules = null)
		{
			validMolecules.Clear();
			foreach (Molecule m in potentialMolecules)
			{
				if ((excludedMolecules == null || !excludedMolecules.Contains( m )) && MoleculeIsValid( m ))
				{
					validMolecules.Add( m );
				}
			}
			if (validMolecules.Count > 0)
			{
				return PickFromValidMolecules();
			}
			return null;
		}

		protected virtual bool MoleculeIsValid (Molecule other)
		{
			return other.NotBoundToType( molecule.type );
		}

		protected virtual Molecule PickFromValidMolecules ()
		{
			return validMolecules[validMolecules.GetRandomIndex()];
		}
	}
}
