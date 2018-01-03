using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public delegate void BindingEvent (MoleculeBinder binder);

	public class MoleculeBinder : MolecularComponent
	{
		public FinderConditional moleculeFinder;
		public BindingCriteria thisCriteria;
		public Vector3 bindingPosition;
		public Vector3 bindingRotation;
		public MoleculeBinder boundBinder;
		public event BindingEvent OnBind;
		public event BindingEvent OnRelease;

		public bool IsAvailableMatch (BindingCriteria _criteria)
		{
			return boundBinder == null && thisCriteria.MatchesOther( _criteria );
		}

		// --------------------------------------------------------------------------------------------------- Bind

		public void Bind ()
		{
			MoleculeBinder _binder = GetBinderToBind();
			if (_binder != null)
			{
				DoBind( _binder );
			}
		}

		protected virtual MoleculeBinder GetBinderToBind ()
		{
			return moleculeFinder.lastBinderFound;
		}

		protected virtual void DoBind (MoleculeBinder otherBinder)
		{
			boundBinder = otherBinder;
			boundBinder.boundBinder = this;

			if (OnBind != null)
			{
				OnBind( boundBinder );
			}
		}

		// --------------------------------------------------------------------------------------------------- Release

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
			MoleculeBinder releasingBinder = boundBinder;
			boundBinder.boundBinder = null;
			boundBinder = null;

			if (OnRelease != null)
			{
				OnRelease( releasingBinder );
			}
		}
	}
}
