using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MotorProteins;

namespace AICS
{
	public class AmbientSprite : MonoBehaviour 
	{
		public bool randomMotion = false;
		public bool animated = false;
		public bool updateEveryFrame = false;
		public float updateInterval = 1f;
		public bool reverseZ = false;
		public bool randomWalkAnimation = false;

		float lastTime;
		int forwardAnimationHash;
		int reverseAnimationHash;
		float lastSwitchTime;
		float switchInterval;

		Animator _animator;
		Animator animator
		{
			get
			{
				if (_animator == null)
				{
					_animator = GetComponentInChildren<Animator>();
				}
				return _animator;
			}
		}

		void Start () 
		{
//			Vector3 toCamera = Camera.main.transform.position - transform.position;
//			transform.rotation = Quaternion.AngleAxis( Random.Range( 0, 359f ), toCamera ) * Quaternion.LookRotation( toCamera );
			lastTime = Time.time;

			if (animated)
			{
				StartAnimation();
			}
		}
		string stateName;
		float time;
		public void StartAnimation ()
		{
			stateName = Random.value < 0.5f ? "forward" : "reverse";
			time = Random.value;
			forwardAnimationHash = Animator.StringToHash( "forward" );
			reverseAnimationHash = Animator.StringToHash( "reverse" );
			animator.Play( stateName, -1, time );
			lastSwitchTime = Time.time;
			switchInterval = Random.Range( 0.2f, 1f );
		}

		public void ReverseAnimation ()
		{
			AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo( 0 );
			animator.Play( state.IsName( "forward" ) ? reverseAnimationHash : forwardAnimationHash, -1, 1f - state.normalizedTime );
			lastSwitchTime = Time.time;
			switchInterval = Random.Range( 0.2f, 1f );
		}

		public void DoUpdate ()
		{
			if (updateEveryFrame || (Time.time - lastTime >= updateInterval))
			{
				LookAtCamera();
				lastTime = Time.time;

				if (randomMotion)
				{
					MoveRandomly();
				}

				if (randomWalkAnimation && Time.time - lastSwitchTime > switchInterval)
				{
					ReverseAnimation();
				}
			}
		}

		void LookAtCamera ()
		{
			if (reverseZ)
			{
				transform.LookAt( transform.position + (transform.position - Camera.main.transform.position) );
			}
			else 
			{
				transform.LookAt( Camera.main.transform.position );
			}
		}

		Vector3 startMovePosition;
		Vector3 goalMovePosition;
		float startMovingNanoseconds;
		float moveDuration;
		float moveSpeed = 0.00005f;
		float meanStepSize = 2f;
		float moveRandomness = 0.9f;
		public float radius;

		void MoveRandomly () 
		{
			float t = (MolecularEnvironment.Instance.nanosecondsSinceStart - startMovingNanoseconds) / moveDuration;
			if (MolecularEnvironment.Instance.nanosecondsSinceStart == 0 || t >= 1f)
			{
				StartMove();
			}
			else
			{
				AnimateMove( t );
			}
		}

		void StartMove ()
		{
			startMovePosition = transform.position;
			goalMovePosition = transform.position + Helpers.GetRandomVector( Helpers.SampleExponentialDistribution( meanStepSize ) );
			moveDuration = Vector3.Distance( startMovePosition, goalMovePosition ) / moveSpeed;
			startMovingNanoseconds = MolecularEnvironment.Instance.nanosecondsSinceStart;

			if (moveDuration <= MolecularEnvironment.Instance.nanosecondsPerStep)
			{
				AnimateMove( 1f );
			}
		}

		void AnimateMove (float t) 
		{
			Vector3 moveStep = Vector3.Lerp( startMovePosition, goalMovePosition, Mathf.Clamp( t, 0, 1f ) ) - transform.position;
			moveStep += Helpers.GetRandomVector( moveRandomness * moveStep.magnitude );

			if ( Vector3.Magnitude( transform.position + moveStep ) > radius )
			{
				transform.position -= moveStep;
			}
			else
			{
				transform.position += moveStep;
			}
		}
	}
}