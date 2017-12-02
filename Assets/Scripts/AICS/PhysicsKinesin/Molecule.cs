using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.PhysicsKinesin
{
	public abstract class Molecule : MonoBehaviour
	{
		public bool isBusy;
		public bool shouldHide;
		public bool hidden;
		public Transform parent;
		public float mass;

		Rigidbody _body;
		public Rigidbody body
		{
			get {
				if (_body == null)
				{
					_body = GetComponent<Rigidbody>();
				}
				return _body;
			}
		}

		void Update ()
		{
			if (!isBusy && shouldHide)
			{
				shouldHide = false;
//				hidden = true;
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
			DoReset();
			ToggleHidden( false );
		}

		protected abstract void DoReset ();
	}
}
