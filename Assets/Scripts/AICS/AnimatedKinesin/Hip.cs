using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AnimatedKinesin
{
	public class Hip : MonoBehaviour 
	{
		RandomMover _mover;
		RandomMover mover
		{
			get
			{
				if (_mover == null)
				{
					_mover = GetComponent<RandomMover>();
					if (_mover == null)
					{
						_mover = gameObject.AddComponent<RandomMover>();
					}
				}
				return _mover;
			}
		}

		void FixedUpdate () 
		{
			mover.Rotate();
			mover.Move();
		}
	}
}