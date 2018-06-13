using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins.Kinesin
{
	public class Necklinker : LinkerComponentMolecule 
	{
        public float positionAlongChain = 0.5f;
        public Transform motor;
        public Transform hips;

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
//			Jitter( 0.1f );
			PositionBetweenParents();
//			PlaceVisualization();
		}

		void LateUpdate ()
		{
			Jitter( 0.1f );
			PositionBetweenParents();
		}

		void PositionBetweenParents ()
		{
            if (motor != null && hips != null)
			{
                SetPosition( positionAlongChain * motor.position + (1f - positionAlongChain) * hips.position );
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

		protected override void InteractWithBindingPartners () { }

		public override void DoCustomReset () { }
	}
}
