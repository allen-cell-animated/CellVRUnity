using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	[System.Serializable]
	public class BindingSiteAngle : System.IComparable<BindingSiteAngle>
	{
		public BindingSite bindingSite;
		public float angle;

		public BindingSiteAngle (BindingSite _bindingSite, float _angle)
		{
			bindingSite = _bindingSite;
			angle = _angle;
		}

		public int CompareTo (BindingSiteAngle other)
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

	public class FrontFinderConditional : FinderConditional 
	{
		public Direction forwardDirection = Direction.forward;
		public Direction upDirection = Direction.up;
		public Molecule centerMolecule;

		List<BindingSiteAngle> graphedBinders = new List<BindingSiteAngle>();

		protected override BindingSite PickFromValidBindingSites ()
		{
			return GetFrontBindingSite();
		}

		protected BindingSite GetFrontBindingSite ()
		{
			GraphMolecules();
			return graphedBinders[graphedBinders.GetExponentialRandomIndex()].bindingSite;
		}

		void GraphMolecules ()
		{
			foreach (BindingSite site in validBindingSites)
			{
				graphedBinders.Add( new BindingSiteAngle( site, GetMoleculeAngleFromForward( site.molecule ) ) );
			}
			graphedBinders.Sort();
		}

		float GetMoleculeAngleFromForward (Molecule _molecule)
		{
			// Get angle in degrees from forward vector projected on plane passing through transform and perpendicular to up vector
			Vector3 toMolecule = _molecule.transform.position - centerMolecule.transform.position;
			Vector3 normal = Helpers.GetLocalDirection( upDirection, centerMolecule.transform );
			Vector3 projectionToNormal = Vector3.Dot( toMolecule, normal ) * normal;
			Vector3 moleculeDirectionInMotorPlane = (toMolecule - projectionToNormal).normalized;

			return Mathf.Acos( Vector3.Dot( Helpers.GetLocalDirection( forwardDirection, centerMolecule.transform ), moleculeDirectionInMotorPlane ) ) * Mathf.Rad2Deg;
		}
	}
}
