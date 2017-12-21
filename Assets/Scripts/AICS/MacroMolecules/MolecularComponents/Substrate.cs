﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class Substrate : BindHandler
	{
		MoleculeBinder currentBinder;

		protected override void OnBind (MoleculeBinder binder)
		{
			if (isEnabled)
			{
				currentBinder = binder;
				ParentToBoundMolecule( binder.molecule );
				SetToBindingOrientation( binder );
			}
		}

		protected override void OnRelease (MoleculeBinder binder)
		{
			if (isEnabled && binder == currentBinder)
			{
				UnParentFromReleasedMolecule( binder.molecule );
				MoveAwayFromReleasedMolecule( binder.molecule );
				currentBinder = null;
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

		public virtual void SetToBindingOrientation (MoleculeBinder binder)
		{
			if (molecule.polymer == null)
			{
				molecule.transform.position = binder.molecule.transform.TransformPoint( binder.boundBinder.bindingPosition );
				molecule.transform.rotation = binder.molecule.transform.rotation * Quaternion.Euler( binder.boundBinder.bindingRotation );
			}
			else
			{
				molecule.polymer.SetToBindingOrientation( binder );
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
			}
		}

		public void ResetToBindingOrientation ()
		{
			SetToBindingOrientation( currentBinder );
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