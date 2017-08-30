using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins.Kinesin
{
	public class Necklinker : LinkerComponentMolecule 
	{
		public override bool bound
		{
			get
			{
				return false;
			}
		}

		void Start ()
		{
			CalculateDistanceFromParents();
		}

		void CalculateDistanceFromParents ()
		{
			float d1 = 0, d2 = 0;
			if (transform.parent != null)
			{
				d1 = Vector3.Distance( transform.position, transform.parent.position );
			}
			if (secondParent != null)
			{
				d2 = Vector3.Distance( transform.position, secondParent.position );
			}
			minDistanceFromParent = Mathf.Min( d1, d2 );
			maxDistanceFromParent = Mathf.Max( d1, d2 );
		}

		public override void DoCustomSimulation ()
		{
			Jitter( 0.1f );
			RelaxBetweenParents();
		}

		public override void DoRandomWalk ()
		{
			Move();
		}

		protected override void InteractWithCollidingMolecules () { }

		public override void DoCustomReset () { }
	}
}
