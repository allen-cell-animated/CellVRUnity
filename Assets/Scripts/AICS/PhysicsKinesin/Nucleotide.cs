using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.PhysicsKinesin
{
	public class Nucleotide : Molecule
	{
		public bool isATP;
		public GameObject ATP;
		public GameObject ADP;
		public GameObject Pi;

		Collider _collider;
		Collider theCollider
		{
			get {
				if (_collider == null)
				{
					_collider = GetComponent<Collider>();
				}
				return _collider;
			}
		}

		RandomForces _randomForces;
		RandomForces randomForces
		{
			get {
				if (_randomForces == null)
				{
					_randomForces = GetComponent<RandomForces>();
				}
				return _randomForces;
			}
		}

		void Start ()
		{
			DoReset();
		}

		public void Hydrolyze ()
		{
			isATP = false;

			DestroyDisplay( ATP );

			GameObject ADPprefab = Resources.Load( "Nucleotides/ADP" ) as GameObject;
			ADP = Instantiate( ADPprefab, transform ) as GameObject;
			ADP.transform.localPosition = Vector3.zero;
			ADP.transform.localRotation = Quaternion.identity;

			GameObject PiPrefab = Resources.Load( "Nucleotides/Pi" ) as GameObject;
			Pi = Instantiate( PiPrefab, transform ) as GameObject;
			Pi.transform.localPosition = Vector3.zero;
			Pi.transform.localRotation = Quaternion.identity;
		}

		public void ReleasePi ()
		{
			if (Pi != null)
			{
				Pi.transform.SetParent( parent );
				Pi.GetComponent<RandomForces>().enabled = true;
				Pi.GetComponent<Rigidbody>().isKinematic = false;
				Invoke( "DestroyPi", 5f );
			}
		}

		void DestroyPi ()
		{
			DestroyDisplay( Pi );
		}

		protected override void Hide ()
		{
			theCollider.enabled = randomForces.enabled = false;
			SetDisplayActive( ATP, false );
			SetDisplayActive( ADP, false );
			SetDisplayActive( Pi, false );
		}

		protected override void Show ()
		{
			theCollider.enabled = randomForces.enabled = true;
			SetDisplayActive( ATP, true );
			SetDisplayActive( ADP, true );
			SetDisplayActive( Pi, true );
		}

		void SetDisplayActive (GameObject display, bool active)
		{
			if (display != null)
			{
				display.SetActive( active );
			}
		}

		protected override void DoReset ()
		{
			isATP = true;

			if (ATP == null)
			{
				GameObject ATPprefab = Resources.Load( "Nucleotides/ATP" ) as GameObject;
				ATP = Instantiate( ATPprefab, transform ) as GameObject;
				ATP.transform.localPosition = Vector3.zero;
				ATP.transform.localRotation = Quaternion.identity;
			}

			DestroyDisplay( ADP );
			DestroyDisplay( Pi );
		}

		void DestroyDisplay (GameObject display)
		{
			if (display != null)
			{
				Destroy( display );
			}
		}

		public void Restart ()
		{
			randomForces.DoReset();
		}
	}
}