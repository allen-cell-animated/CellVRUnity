using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class MoleculeBinder : MolecularComponent, IBind
	{
		public MoleculeFinderConditional moleculeFinder;
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

		public void Bind ()
		{
			IBind otherBinder = GetMoleculeToBind();
			if (otherBinder != null)
			{
				DoBind( otherBinder );
			}
		}

		protected virtual IBind GetMoleculeToBind ()
		{
			return moleculeFinder.lastBinderFound;
		}

		protected virtual void DoBind (IBind otherBinder)
		{
			boundMoleculeBinder = otherBinder;
			boundMoleculeBinder.boundMoleculeBinder = this;

			if (parentToBoundMolecule)
			{
				molecule.ParentToBoundMolecule( boundMoleculeBinder.molecule );
				MoveMoleculeToBindingPosition();
			}
			else if (boundMoleculeBinder.parentToBoundMolecule)
			{
				boundMoleculeBinder.molecule.ParentToBoundMolecule( molecule );
				boundMoleculeBinder.MoveMoleculeToBindingPosition();
			}
		}

		public void MoveMoleculeToBindingPosition ()
		{
			transform.rotation = boundMoleculeBinder.molecule.transform.rotation * Quaternion.Euler( bindingRotation );
			molecule.SetPosition( boundMoleculeBinder.molecule.transform.TransformPoint( bindingPosition ) );
		}

		public void Release ()
		{
			if (boundMoleculeBinder != null && ReadyToRelease())
			{
				DoRelease();
			}
		}

		protected virtual bool ReadyToRelease ()
		{
			return true;
		}

		protected virtual void DoRelease ()
		{
			Vector3 awayFromBoundMolecule = 3f * (transform.position - boundMoleculeBinder.molecule.transform.position).normalized;

			if (parentToBoundMolecule)
			{
				molecule.UnParentFromBoundMolecule();
			}
			else if (boundMoleculeBinder.parentToBoundMolecule)
			{
				boundMoleculeBinder.molecule.UnParentFromBoundMolecule();
			}

			boundMoleculeBinder.boundMoleculeBinder = null;
			boundMoleculeBinder = null;

			molecule.MoveIfValid( awayFromBoundMolecule );
		}
	}
}
