using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class Polymer : Substrate
	{
		public Molecule rootMonomer;
		public List<Molecule> monomers = new List<Molecule>();

		List<Molecule> boundMonomerMolecules = new List<Molecule>();

		public override void ParentToBoundMolecule (Molecule _bindingMolecule)
		{
			if (boundMonomerMolecules.Count == 0)
			{
				base.ParentToBoundMolecule( _bindingMolecule );
			}
		}

		public override void UnParentFromReleasedMolecule (Molecule _releasingMolecule)
		{
			if (boundMonomerMolecules.Count == 1)
			{
				base.UnParentFromReleasedMolecule( _releasingMolecule );
			}
			else if (boundMonomerMolecules.Count > 1 && transform.parent == _releasingMolecule)
			{
				GetBoundMonomerMolecules();
				boundMonomerMolecules.Remove( _releasingMolecule );
				base.ParentToBoundMolecule( boundMonomerMolecules[boundMonomerMolecules.GetRandomIndex()] );
			}
		}

		public override void SetToBindingOrientation (BindingSite bindingSite)
		{
			int n = transform.childCount;
			Transform[] children = new Transform[n];
			Vector3[] positions = new Vector3[n];
			Quaternion[] rotations = new Quaternion[n];

			int i = 0;
			foreach (Transform child in transform)
			{
				children[i] = child;
				positions[i] = child.position;
				rotations[i] = child.rotation;
				i++;
			}

			base.SetToBindingOrientation( bindingSite );

			for (int j = 0; j < n; j++)
			{
				children[j].position = positions[j];
				children[j].rotation = rotations[j];
			}
		}

		public void UpdateParentScheme ()
		{
			GetBoundMonomerMolecules();

			if (boundMonomerMolecules.Count == 0)
			{
				if (rootMonomer != null)
				{
					rootMonomer.transform.SetParent( transform );
					ParentAllAttachedMoleculesTo( rootMonomer, null );
				}
			}
			else if (boundMonomerMolecules.Count == 1)
			{
				boundMonomerMolecules[0].transform.SetParent( transform );
				ParentAllAttachedMoleculesTo( boundMonomerMolecules[0], null );
			}
			else if (boundMonomerMolecules.Count > 1)
			{
				Molecule boundMonomerClosestToRoot = GetMoleculeClosestToRoot( boundMonomerMolecules );

				foreach (Leash leash in rootMonomer.leashes)
				{
					Molecule nextToClosest = leash.GetMonomerClosestTo( boundMonomerClosestToRoot );
					if (nextToClosest != null)
					{
						Debug.Log( "leash from root to " + leash.attachedMolecule.name + " ends at " + nextToClosest.name );
						nextToClosest.transform.SetParent( boundMonomerClosestToRoot.transform );
						ParentAllAttachedMoleculesTo( nextToClosest, boundMonomerClosestToRoot ); 
					}
				}

				foreach (Molecule boundMonomer in boundMonomerMolecules)
				{
					boundMonomer.transform.SetParent( transform );
					ParentAllAttachedMoleculesTo( boundMonomer, GetAttachedMoleculeTowardRoot( boundMonomer ) );
				}
			}
		}

		Molecule GetAttachedMoleculeTowardRoot (Molecule parent)
		{
			foreach (Leash leash in parent.leashes)
			{
				if (leash.GetMonomerClosestTo( rootMonomer ) != null)
				{
					return leash.attachedMolecule;
				}
			}
			return null;
		}

		List<Molecule> GetBoundMonomerMolecules ()
		{
			boundMonomerMolecules.Clear();
			foreach (Molecule monomer in monomers)
			{
				if (monomer.bound)
				{
					boundMonomerMolecules.Add( monomer );
				}
			}
			return boundMonomerMolecules;
		}

		Molecule GetMoleculeClosestToRoot (List<Molecule> _monomers)
		{
			int n, min = int.MaxValue - 10;
			Molecule closestMonomer = null;
			foreach (Molecule monomer in _monomers)
			{
				if (monomer == rootMonomer)
				{
					n = 0;
				}
				else 
				{
					foreach (Leash leash in monomer.leashes)
					{
						n = leash.GetMinBranchesToMolecule( rootMonomer );
						if (n < min)
						{
							closestMonomer = monomer;
							min = n;
						}
					}
				}
			}
			return closestMonomer;
		}

		void ParentAllAttachedMoleculesTo (Molecule parent, Molecule excludedMolecule)
		{
			foreach (Leash leash in parent.leashes)
			{
				if (leash.attachedMolecule != excludedMolecule)
				{
					leash.attachedMolecule.transform.SetParent( parent.transform );
					ParentAllAttachedMoleculesTo( leash.attachedMolecule, parent );
				}
			}
		}
	}
}