using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Nucleotide : MonoBehaviour 
	{
		public bool isATP;
		public bool bound = true;
		public float rangeRadius = 10f;
		public float simulationForce = 5f;
		public float minRandomForce = 20f;
		public float maxRandomForce = 50f;
		public float timeBetweenImpulses = 0.1f;

		Motor motor;
		public bool simulating;
		float lastTime = -1f;
		Vector3 bindingPosition = new Vector3( -0.66f, 0.73f, -2.5f );
		Vector3 goalPosition;
		Color atpColor;
		Color adpColor;
		Rigidbody body;

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

		MeshRenderer _meshRenderer;
		MeshRenderer meshRenderer
		{
			get {
				if (_meshRenderer == null)
				{
					_meshRenderer = GetComponent<MeshRenderer>();
				}
				return _meshRenderer;
			}
		}

		public void Init (Motor _motor)
		{
			motor = _motor;
			transform.position = motor.transform.TransformPoint( bindingPosition );
			transform.SetParent( motor.transform );
			atpColor = motor.kinesin.atpColor;
			adpColor = motor.kinesin.adpColor;
		}

		public void StartATPBinding ()
		{
			Debug.Log(motor.name + " Start ATP binding");
			isATP = true;
			transform.position = motor.transform.position + randomPosition;
			goalPosition = motor.transform.TransformPoint( bindingPosition );
			meshRenderer.material.color = atpColor;
			Enable( true );
			simulating = true;
		}

		void BindATP ()
		{
			bound = true;
			Destroy( body );
			transform.position = motor.transform.TransformPoint( bindingPosition );
			transform.SetParent( motor.transform );
			motor.BindATP();
		}

		public void ReleaseADP ()
		{
			if (bound)
			{
				Debug.Log(motor.name + " Release ADP");
				bound = false;
				AddRigidbody();
				transform.SetParent( null );
				goalPosition = motor.transform.position + randomPosition;
				simulating = true;
			}
		}

		void AddRigidbody ()
		{
			body = gameObject.AddComponent<Rigidbody>();
			body.useGravity = false;
			body.mass = 0.1f;
		}

		void TurnOffADP ()
		{
			Enable( false );
		}

		public void Hydrolyze ()
		{
			if (bound)
			{
				Debug.Log(motor.name + " Hydrolyze");
				meshRenderer.material.color = adpColor;
				isATP = false;
			}
		}

		void Update ()
		{
			if (simulating && Time.time - lastTime > timeBetweenImpulses)
			{
				body.velocity = body.angularVelocity = Vector3.zero;

				if (isATP)
				{
					SimulateATP();
				}
				else
				{
					SimulateADP();
				}
				lastTime = Time.time;
			}
		}

		void SimulateATP ()
		{
			if (motor.shouldATPBind && Vector3.Distance( motor.transform.TransformPoint( bindingPosition ), transform.position ) < 2f)
			{
				simulating = false;
				BindATP();
			}
			else
			{
				body.AddForce( simulationForce * (motor.transform.TransformPoint( bindingPosition ) - transform.position) 
					+ Helpers.GetRandomVector( minRandomForce, maxRandomForce ) );
			}
		}

		void SimulateADP ()
		{
			if (Vector3.Distance( motor.transform.position, transform.position ) > rangeRadius - 1f)
			{
				simulating = false;
				TurnOffADP();
			}
			else
			{
				Vector3 toGoal = goalPosition - transform.position;
				float distanceToGoal = Mathf.Min( 1f, Vector3.Magnitude( toGoal ) / 15f );
				body.AddForce( simulationForce * toGoal 
					+ Helpers.GetRandomVector( distanceToGoal * minRandomForce, distanceToGoal * maxRandomForce ) );
			}
		}

		void Enable (bool enable)
		{
			if (enable)
			{
				meshRenderer.enabled = theCollider.enabled = true;
			}
			else
			{
				meshRenderer.enabled = theCollider.enabled = false;
			}
		}

		Vector3 randomPosition
		{
			get {
				float theta = Mathf.PI * Random.Range( 0, 1f );
				float phi = 2f * Mathf.PI * Random.Range( 0, 1f );
				return rangeRadius * new Vector3( Mathf.Sin( theta ) * Mathf.Cos( phi ), Mathf.Sin( theta ) * Mathf.Sin( phi ), Mathf.Cos( theta ) );
			}
		}
	}
}