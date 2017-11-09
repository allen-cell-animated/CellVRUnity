using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MotorProteins;

namespace AICS.MT
{
	public class Vesicle : MonoBehaviour, IWalkSplines
	{
		public float speed = 1f;
		public float jitter = 1f;

		public Microtubule microtubule { get; set; }
		public float t { get; set; }

		void Update () 
		{
			t += speed / (100f * MolecularEnvironment.Instance.timeMultiplier);
			if (t >= 1f)
			{
				t = 0;
			}

			PlaceOnMicrotubule();
		}

		void PlaceOnMicrotubule ()
		{
			transform.position = microtubule.spline.GetPosition( t ) + Helpers.GetRandomVector( jitter / MolecularEnvironment.Instance.timeMultiplier );
		}
	}
}