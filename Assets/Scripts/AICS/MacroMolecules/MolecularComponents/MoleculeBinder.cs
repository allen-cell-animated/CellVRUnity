using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class MoleculeBinder : MolecularComponent
	{
		public FinderConditional moleculeFinder;
		public bool _parentToBoundMolecule;
		public Vector3 bindingPosition;
		public Vector3 bindingRotation;
		public MoleculeBinder boundBinder;

		public virtual MoleculeType typeToBind
		{
			get
			{
				return moleculeFinder.typeToFind;
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
			MoleculeBinder otherBinder = GetMoleculeToBind();
			if (otherBinder != null)
			{
				DoBind( otherBinder );
			}
		}

		protected virtual MoleculeBinder GetMoleculeToBind ()
		{
			return moleculeFinder.lastBinderFound;
		}

		protected virtual void DoBind (MoleculeBinder otherBinder)
		{
			boundBinder = otherBinder;
			boundBinder.boundBinder = this;

			if (parentToBoundMolecule)
			{
				molecule.ParentToBoundMolecule( boundBinder.molecule );
				molecule.SetToBindingOrientation( this );
			}
			else if (boundBinder.parentToBoundMolecule)
			{
				boundBinder.molecule.ParentToBoundMolecule( molecule );
				boundBinder.molecule.SetToBindingOrientation( boundBinder );
			}
		}

		public void Release ()
		{
			if (boundBinder != null && ReadyToRelease())
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
			Vector3 awayFromBoundMolecule = 3f * (transform.position - boundBinder.molecule.transform.position).normalized;

			Molecule releasingMolecule = boundBinder.molecule;
			boundBinder.boundBinder = null;
			boundBinder = null;

			if (parentToBoundMolecule)
			{
				molecule.UnParentFromBoundMolecule( releasingMolecule );
			}
			else if (boundBinder.parentToBoundMolecule)
			{
				boundBinder.molecule.UnParentFromBoundMolecule( molecule );
			}

			molecule.MoveIfValid( awayFromBoundMolecule );
		}
	}
}
