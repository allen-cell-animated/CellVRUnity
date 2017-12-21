using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public enum BindingState
	{
		bound,
		empty
	}

	public class BindingConditional : Conditional
	{
		public MoleculeBinder binder;
		public BindingState requiredState;

		protected override bool DoCheck ()
		{
			return (requiredState == BindingState.bound && binder.boundBinder != null)
				|| (requiredState == BindingState.empty && binder.boundBinder == null);
		}
	}
}