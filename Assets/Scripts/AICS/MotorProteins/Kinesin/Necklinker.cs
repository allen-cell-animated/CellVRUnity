using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins.Kinesin
{
	public class Necklinker : LinkerComponentMolecule 
	{
		Transform _visualization;
		Transform visualization
		{
			get
			{
				if (_visualization == null)
				{
					_visualization = transform.Find( "Capsule" );
				}
				return _visualization;
			}
		}

		public override bool bound
		{
			get
			{
				return false;
			}
		}

		protected override void OnAwake () { }

		public override void DoCustomSimulation ()
		{
			Jitter( 0.1f );
			PositionBetweenParents();
//			PlaceVisualization();
		}

		void PositionBetweenParents ()
		{
			if (transform.parent != null && secondParent != null)
			{
				SetPosition( (transform.parent.position + secondParent.position) / 2f );
			}
		}

		void PlaceVisualization ()
		{
			Vector3 toParent = transform.parent.position - transform.position;
			float length = toParent.magnitude;

			//position
			visualization.transform.position = transform.position + (length / 2f) * toParent.normalized;

			//rotation
			transform.LookAt( transform.parent );

			//scale
			visualization.transform.localScale = new Vector3( 0.5f, Mathf.Max( length - 0.5f / 2f, 0.5f ), 0.5f );
		}

		public override void DoRandomWalk () { }

		protected override void InteractWithCollidingMolecules () { }

		public override void DoCustomReset () { }
	}
}
