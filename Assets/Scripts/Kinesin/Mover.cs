using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Mover : MonoBehaviour 
	{
		Vector3 startPosition;
		Vector3 goalPosition;
		float t;
		float speed;
		bool moving;

		public void MoveToOverDuration (Vector3 _goalPosition, float _duration)
		{
			speed = 1f / _duration;

			MoveTo( _goalPosition );
		}

		public void MoveToWithSpeed (Vector3 _goalPosition, float _speed)
		{
			float distance = Vector3.Distance(transform.position, _goalPosition);
			speed = 1f / (distance / _speed);

			MoveTo( _goalPosition );
		}

		void MoveTo (Vector3 _goalPosition)
		{
			startPosition = transform.position;
			goalPosition = _goalPosition;
			t = 0;
			moving = true;
		}

		void Update () 
		{
			if (moving)
			{
				t += speed * Time.deltaTime;
				if (t >= 1f)
				{
					t = 1f;
					moving = false;
				}

				transform.position = Vector3.Lerp( startPosition, goalPosition, t );
			}
		}
	}
}