using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MotorProteins;

namespace AICS.MT
{
	public class Vesicle : MonoBehaviour
	{
		float speed = 0.03f;
		float jitter = 2f;
		public Microtubule microtubule;
		public float t;

		AmbientSprite _sprite;
		AmbientSprite sprite
		{
			get 
			{
				if (_sprite == null)
				{
					_sprite = GetComponentInChildren<AmbientSprite>();
				}
				return _sprite;
			}
		}

		public void DoUpdate () 
		{
			t -= speed / (100f + 0.9f * MolecularEnvironment.Instance.timeMultiplier);
			if (t <= 0)
			{
				t = 1f;
			}

			sprite.DoUpdate();
			PlaceOnMicrotubule();
			DoAdditionalUpdate();
		}

		void PlaceOnMicrotubule ()
		{
			transform.position = microtubule.spline.GetPosition( t ) + Helpers.GetRandomVector( jitter / (100f + 0.9f * MolecularEnvironment.Instance.timeMultiplier) );
		}

		protected virtual void DoAdditionalUpdate () { }
	}
}