using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
		public UnityEvent OnBind;
		public UnityEvent OnRelease;

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
				boundBinder.DoBind( this );

				BroadcastBind();
				boundBinder.BroadcastBind();
			}
		}

		protected virtual MoleculeBinder GetBinderToBind ()
		{
			return moleculeFinder.lastBinderFound;
		}

		protected virtual void DoBind (MoleculeBinder otherBinder)
		{
			boundBinder = otherBinder;
		}

		protected virtual void BroadcastBind ()
		{
			if (OnBind != null)
			{
				OnBind.Invoke();
			}
		}

		// --------------------------------------------------------------------------------------------------- Release

		public void Release ()
		{
			if (boundBinder != null && ReadyToRelease())
			{
				MoleculeBinder _binder = boundBinder;
				boundBinder.DoRelease();
				DoRelease();

				BroadcastRelease();
				_binder.BroadcastRelease();
			}
		}

		protected virtual bool ReadyToRelease ()
		{
			return true;
		}

		protected virtual void DoRelease ()
		{
			boundBinder = null;
		}

		protected virtual void BroadcastRelease ()
		{
			if (OnRelease != null)
			{
				OnRelease.Invoke();
			}
		}
	}
}
