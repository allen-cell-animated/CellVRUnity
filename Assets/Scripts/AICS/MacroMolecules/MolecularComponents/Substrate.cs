using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public abstract class Substrate : MolecularComponent, IHandleBinds
	{
		List<MoleculeBinder> subscribedBinders;
		MoleculeBinder currentBinder;

		public void SubscribeToBindEvents (List<MoleculeBinder> binders)
		{
			foreach (MoleculeBinder binder in binders)
			{
				binder.OnBind += OnBind;
				binder.OnRelease += OnRelease;
			}
			subscribedBinders = binders;
		}

		void OnDisable ()
		{
			foreach (MoleculeBinder binder in subscribedBinders)
			{
				binder.OnBind -= OnBind;
				binder.OnRelease -= OnRelease;
			}
		}

		protected virtual void OnBind (MoleculeBinder binder)
		{
			currentBinder = binder;
			ParentToBoundMolecule( binder.molecule );
			SetToBindingOrientation( binder );
		}

		protected virtual void OnRelease (MoleculeBinder binder)
		{
			if (binder == currentBinder)
			{
				UnParentFromReleasedMolecule( binder.molecule );
				MoveAwayFromReleasedMolecule( binder.molecule );
				currentBinder = null;
			}
		}

		public virtual void ParentToBoundMolecule (Molecule _bindingMolecule)
		{
			transform.SetParent( _bindingMolecule.transform );
		}

		public virtual void SetToBindingOrientation (MoleculeBinder binder)
		{
			molecule.transform.position = binder.molecule.transform.TransformPoint( binder.boundBinder.bindingPosition );
			molecule.transform.rotation = binder.molecule.transform.rotation * Quaternion.Euler( binder.boundBinder.bindingRotation );
		}

		public void ResetToBindingOrientation ()
		{
			SetToBindingOrientation( currentBinder );
		}

		public virtual void UnParentFromReleasedMolecule (Molecule _releasingMolecule)
		{
			transform.SetParent( null );
		}

		protected virtual void MoveAwayFromReleasedMolecule (Molecule _releasingMolecule)
		{
			molecule.MoveIfValid( 3f * (transform.position - _releasingMolecule.transform.position).normalized );
		}
	}
}