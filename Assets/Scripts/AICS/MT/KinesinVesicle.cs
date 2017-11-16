using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MotorProteins.Kinesin;

namespace AICS.MT
{
	public class KinesinVesicle : Vesicle
	{
		public KinesinSpawner spawner;
		public Kinesin kinesin;
		public float normalRotation;
		public Transform sprite;

		protected override void DoAdditionalUpdate () 
		{
			if (t > spawner.kinesinRange.x && t < spawner.kinesinRange.y)
			{
				spawner.ConvertToKinesin( this );
			}
		}
	}
}