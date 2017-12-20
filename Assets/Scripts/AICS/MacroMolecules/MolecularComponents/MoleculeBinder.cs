using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public delegate void BindingEvent (BindingSite bindingSite);

	public class MoleculeBinder : MolecularComponent
	{
		public FinderConditional moleculeFinder;
		public Vector3 bindingPosition;
		public Vector3 bindingRotation;
		public BindingSite bindingSite;
		public event BindingEvent OnBind;
		public event BindingEvent OnRelease;

		public virtual MoleculeType typeToBind
		{
			get
			{
				return moleculeFinder.typeToFind;
			}
		}

		// --------------------------------------------------------------------------------------------------- Bind

		public void Bind ()
		{
			BindingSite _bindingSite = GetSiteToBind();
			if (_bindingSite != null)
			{
				DoBind( _bindingSite );
			}
		}

		protected virtual BindingSite GetSiteToBind ()
		{
			return moleculeFinder.lastBindingSiteFound;
		}

		protected virtual void DoBind (BindingSite _bindingSite)
		{
			bindingSite = _bindingSite;
			bindingSite.boundBinder = this;

			if (OnBind != null)
			{
				OnBind( bindingSite );
			}
		}

		// --------------------------------------------------------------------------------------------------- Release

		public void Release ()
		{
			if (bindingSite != null && ReadyToRelease())
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
			BindingSite releasingSite = bindingSite;
			bindingSite.boundBinder = null;
			bindingSite = null;

			if (OnRelease != null)
			{
				OnRelease( releasingSite );
			}
		}
	}
}
