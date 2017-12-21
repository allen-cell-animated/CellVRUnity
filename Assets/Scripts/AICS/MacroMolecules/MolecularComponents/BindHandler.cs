using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public abstract class BindHandler : MolecularComponent
	{
		public bool subscribeToAll = true;
		List<MoleculeBinder> subscribedBinders = new List<MoleculeBinder>();

		public void SubscribeToAllBinders (List<MoleculeBinder> binders)
		{
			if (subscribeToAll)
			{
				foreach (MoleculeBinder binder in binders)
				{
					SubscribeToBinder( binder );
				}
			}
		}

		public void SubscribeToBinder (MoleculeBinder binder)
		{
			binder.OnBind += OnBind;
			binder.OnRelease += OnRelease;
			subscribedBinders.Add( binder );
		}

		void OnDisable ()
		{
			foreach (MoleculeBinder binder in subscribedBinders)
			{
				binder.OnBind -= OnBind;
				binder.OnRelease -= OnRelease;
			}
		}

		protected abstract void OnBind (MoleculeBinder binder);

		protected abstract void OnRelease (MoleculeBinder binder);
	}
}
