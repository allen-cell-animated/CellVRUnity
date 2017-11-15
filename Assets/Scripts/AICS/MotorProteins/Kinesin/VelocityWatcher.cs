using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins.Kinesin
{
	public class VelocityWatcher : MonoBehaviour 
	{
		int n = 4;
		Vector3[] positions;

		public Vector3 displacement
		{
			get
			{
				return (positions[n - 1] - positions[0]) / (n - 1f);
			}
		}

		public Vector3 velocity
		{
			get
			{
				Vector3 v = Vector3.zero;
				for (int i = 0; i < n - 1; i++)
				{
					v += positions[i + 1] - positions[i];
				}
				return v / (n - 1);
			}
		}

		public Vector3 acceleration
		{
			get
			{
				Vector3 a = Vector3.zero;
				for (int i = 0; i < n - 2; i++)
				{
					a += positions[i + 2] - 2f * positions[i + 1] + positions[i];
				}
				return a / (n - 2);
			}
		}

		void Start ()
		{
			positions = new Vector3[n];
			foreach (Vector3 position in positions)
			{
				position = transform.position;
			}
		}

		void Update () 
		{
			for (int i = 0; i < n - 1; i++)
			{
				positions[i] = positions[i + 1];
			}
			positions[n - 1] = transform.position;
		}
	}
}