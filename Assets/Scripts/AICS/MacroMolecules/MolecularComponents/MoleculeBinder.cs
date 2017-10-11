using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class MoleculeBinder : MolecularComponent, IBind
	{
		public MoleculeFinder moleculeFinder;
		public bool _parentToBoundMolecule;
		public Vector3 bindingPosition;
		public Vector3 bindingRotation;
		public IBind _boundMoleculeBinder;

		public MoleculeType typeToBind
		{
			get
			{
				return moleculeFinder.typeToFind;
			}
		}

		public Molecule thisMolecule
		{
			get
			{
				return molecule;
			}
		}

		public IBind boundMoleculeBinder
		{
			get
			{
				return _boundMoleculeBinder;
			}
			set
			{
				_boundMoleculeBinder = value;
			}
		}

		public bool parentToBoundMolecule
		{
			get
			{
				return _parentToBoundMolecule;
			}
		}

		public bool Bind ()
		{
			IBind otherBinder = GetMoleculeToBind();

			if (otherBinder != null)
			{
				DoBind( otherBinder );
				return true;
			}
			return false;
		}

		protected virtual IBind GetMoleculeToBind ()
		{
			return moleculeFinder.Find();
		}

		protected virtual void DoBind (IBind otherBinder)
		{
			boundMoleculeBinder = otherBinder;
			boundMoleculeBinder.boundMoleculeBinder = this;

			if (parentToBoundMolecule)
			{
				molecule.ParentToBoundMolecule( boundMoleculeBinder.thisMolecule );
			}
			else if (boundMoleculeBinder.parentToBoundMolecule)
			{
				boundMoleculeBinder.thisMolecule.ParentToBoundMolecule( molecule );
			}

			MoveMoleculeToBindingPosition();
		}

		public void MoveMoleculeToBindingPosition ()
		{
			transform.rotation = boundMoleculeBinder.thisMolecule.transform.rotation * Quaternion.Euler( bindingRotation );
			molecule.SetPosition( boundMoleculeBinder.thisMolecule.transform.TransformPoint( bindingPosition ) );
		}

		public bool Release ()
		{
			if (boundMoleculeBinder != null)
			{
				DoRelease();
				return true;
			}
			return false;
		}

		protected virtual void DoRelease ()
		{
			Vector3 awayFromBoundMolecule = 3f * (transform.position - boundMoleculeBinder.thisMolecule.transform.position).normalized;

			if (parentToBoundMolecule)
			{
				molecule.UnParentFromBoundMolecule();
			}
			else if (boundMoleculeBinder.parentToBoundMolecule)
			{
				boundMoleculeBinder.thisMolecule.UnParentFromBoundMolecule();
			}

			boundMoleculeBinder.boundMoleculeBinder = null;
			boundMoleculeBinder = null;

			molecule.MoveIfValid( awayFromBoundMolecule );
		}
	}
}
