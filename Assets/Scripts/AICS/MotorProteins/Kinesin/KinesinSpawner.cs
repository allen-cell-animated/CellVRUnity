using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MT;

namespace AICS.MotorProteins.Kinesin
{
	public class KinesinSpawner : MicrotubuleSpawner 
	{
		public KinesinParameterInput parameterSetter;

		protected override void SetupObject (IWalkSplines obj, float t)
		{
			Kinesin kinesin = obj as Kinesin;
			parameterSetter.InitKinesin( kinesin );
			spawnedObjects.Add( kinesin.hips.gameObject );
		}

		protected override void DoCustomDestruction (GameObject obj) 
		{
			Kinesin kinesin = obj.GetComponent<Hips>().kinesin;
			kinesin.atpGenerator.DestroyAll();
			Destroy( kinesin.gameObject );
		}
	}
}