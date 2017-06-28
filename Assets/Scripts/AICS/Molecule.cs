using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public abstract class Molecule : MonoBehaviour
	{
		public bool isBusy;
		public bool shouldHide;
		public bool hidden;
		public Transform parent;

		void Update ()
		{
			if (!isBusy && shouldHide)
			{
				shouldHide = false;
				hidden = true;
				ToggleHidden( true );
			}
		}

		void ToggleHidden (bool hide)
		{
			if (hide)
			{
				Hide();
			}
			else
			{
				Show();
			}
			hidden = hide;
		}

		protected abstract void Hide ();

		protected abstract void Show ();

		public void Regenerate ()
		{
			Reset();
			ToggleHidden( false );
		}

		protected abstract void Reset ();
	}
}
