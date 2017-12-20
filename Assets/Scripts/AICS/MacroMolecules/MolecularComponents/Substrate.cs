using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class Substrate : MolecularComponent, IHandleBinds
	{
		List<MoleculeBinder> subscribedBinders = new List<MoleculeBinder>();
		BindingSite currentBindingSite;

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

		protected virtual void OnBind (BindingSite bindingSite)
		{
			currentBindingSite = bindingSite;
			ParentToBoundMolecule( bindingSite.molecule );
			SetToBindingOrientation( bindingSite );
		}

		protected virtual void OnRelease (BindingSite bindingSite)
		{
			if (bindingSite == currentBindingSite)
			{
				UnParentFromReleasedMolecule( bindingSite.molecule );
				MoveAwayFromReleasedMolecule( bindingSite.molecule );
				currentBindingSite = null;
			}
		}

		public virtual void ParentToBoundMolecule (Molecule _bindingMolecule)
		{
			if (molecule.polymer == null)
			{
				transform.SetParent( _bindingMolecule.transform );
			}
			else
			{
				molecule.polymer.ParentToBoundMolecule( _bindingMolecule );
				molecule.polymer.UpdateParentScheme();
			}
		}

		public virtual void SetToBindingOrientation (BindingSite bindingSite)
		{
			if (molecule.polymer == null)
			{
				molecule.transform.position = bindingSite.molecule.transform.TransformPoint( bindingSite.boundBinder.bindingPosition );
				molecule.transform.rotation = bindingSite.molecule.transform.rotation * Quaternion.Euler( bindingSite.boundBinder.bindingRotation );
			}
			else
			{
				molecule.polymer.SetToBindingOrientation( bindingSite );
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
			}
		}

		public void ResetToBindingOrientation ()
		{
			SetToBindingOrientation( currentBindingSite );
		}

		public virtual void UnParentFromReleasedMolecule (Molecule _releasingMolecule)
		{
			if (molecule.polymer == null)
			{
				transform.SetParent( null );
			}
			else
			{
				molecule.polymer.UnParentFromReleasedMolecule( _releasingMolecule );
				molecule.polymer.UpdateParentScheme();
			}
			MoveAwayFromReleasedMolecule( _releasingMolecule );
		}

		protected virtual void MoveAwayFromReleasedMolecule (Molecule _releasingMolecule)
		{
			molecule.MoveIfValid( 3f * (transform.position - _releasingMolecule.transform.position).normalized );
		}
	}
}