using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AICS.MT;

namespace AICS.MotorProteins
{
	[System.Serializable]
	public class TubulinAngle : IComparable<TubulinAngle>
	{
		public Tubulin molecule;
		public float angle;

		public TubulinAngle (Tubulin _molecule, float _angle)
		{
			molecule = _molecule;
			angle = _angle;
		}

		public int CompareTo (TubulinAngle other)
		{
			if (other.angle > angle)
			{
				return -1;
			}
			else if (other.angle == angle)
			{
				return 0;
			}
			else 
			{
				return 1;
			}
		}
	}

	[System.Serializable]
	public class TubulinGraph
	{
		public List<TubulinAngle> molecules = new List<TubulinAngle>();

		Direction forwardDirection;
		Direction upDirection;
		Transform lastCenterTransform;

		public bool empty 
		{
			get
			{
				return molecules.Count < 1;
			}
		}

		public TubulinGraph (Direction _forwardDirection, Direction _upDirection)
		{
			forwardDirection = _forwardDirection;
			upDirection = _upDirection;
		}

		public void SetMolecules (List<Tubulin> _molecules, Transform centerTransform)
		{
			Clear();
			lastCenterTransform = centerTransform;
			float angle;
			foreach (Tubulin m in _molecules)
			{
				angle = GetMoleculeAngleFromForward( m );
                if (angle == 0)
                {
                    molecules.Add( new TubulinAngle( m, angle ) );
                }
			}
			molecules.Sort();
		}

		// Get angle in degrees from forward vector projected on plane passing through transform and perpendicular to up vector
		float GetMoleculeAngleFromForward (Molecule molecule)
		{
			Vector3 toMolecule = molecule.transform.position - lastCenterTransform.position;
			Vector3 normal = Helpers.GetLocalDirection( upDirection, lastCenterTransform );
			Vector3 projectionToNormal = Vector3.Dot( toMolecule, normal ) * normal;
			Vector3 moleculeDirectionInMotorPlane = (toMolecule - projectionToNormal).normalized;

            float dot = Vector3.Dot( Helpers.GetLocalDirection( forwardDirection, lastCenterTransform ), moleculeDirectionInMotorPlane );
            if (dot > 1f - float.Epsilon)
            {
                return 0;
            }
            else if (dot < -1f + float.Epsilon)
            {
                return 180f;
            }
            return Mathf.Acos( dot ) * Mathf.Rad2Deg;
		}

		public void Clear ()
		{
			molecules.Clear();
		}
	}
}