using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MotorProteins;

namespace AICS.MT
{
	public class Vesicle : MonoBehaviour
	{
		public float speed = 1f;
		public float jitter = 1f;

		public Microtubule microtubule { get; set; }
		public float t { get; set; }

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
			t += speed / (100f * MolecularEnvironment.Instance.timeMultiplier);
			if (t >= 1f)
			{
				t = 0;
				sprite.gameObject.SetActive( false );
			}

			sprite.DoUpdate();
			PlaceOnMicrotubule();

			if (t == 0)
			{
				sprite.gameObject.SetActive( true );
			}
		}

		void PlaceOnMicrotubule ()
		{
			transform.position = microtubule.spline.GetPosition( t ) + Helpers.GetRandomVector( jitter / MolecularEnvironment.Instance.timeMultiplier );
		}
	}
}