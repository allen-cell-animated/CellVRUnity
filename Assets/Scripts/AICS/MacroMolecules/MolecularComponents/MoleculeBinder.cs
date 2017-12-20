using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public delegate void BindingEvent (MoleculeBinder binder);

	public class MoleculeBinder : MolecularComponent
	{
		public FinderConditional moleculeFinder;
		public Vector3 bindingPosition;
		public Vector3 bindingRotation;
		public MoleculeBinder boundBinder;
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
			MoleculeBinder otherBinder = GetMoleculeToBind();
			if (otherBinder != null)
			{
				DoBind( otherBinder );
				otherBinder.DoBind( this );
			}
		}

		protected virtual MoleculeBinder GetMoleculeToBind ()
		{
			return moleculeFinder.lastBinderFound;
		}

		protected virtual void DoBind (MoleculeBinder otherBinder)
		{
			boundBinder = otherBinder;

			if (OnBind != null)
			{
				OnBind( otherBinder );
			}
		}

		// --------------------------------------------------------------------------------------------------- Release

		public void Release ()
		{
			if (boundBinder != null && ReadyToRelease())
			{
				boundBinder.DoRelease();
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
			boundBinder = null;

			if (OnRelease != null)
			{
				OnRelease( releasingBinder );
			}
		}
	}
}
