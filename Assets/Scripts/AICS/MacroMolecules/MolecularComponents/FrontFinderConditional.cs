using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	[System.Serializable]
	public class BinderAngle : System.IComparable<BinderAngle>
	{
		public MoleculeBinder binder;
		public float angle;

		public BinderAngle (MoleculeBinder _binder, float _angle)
		{
			binder = _binder;
			angle = _angle;
		}

		public int CompareTo (BinderAngle other)
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

		List<BinderAngle> graphedBinders = new List<BinderAngle>();

		protected override MoleculeBinder PickFromValidBinders ()
		{
			return GetFrontBinder();
		}

		protected MoleculeBinder GetFrontBinder ()
		{
			GraphMolecules();
			return graphedBinders[graphedBinders.GetExponentialRandomIndex()].binder;
		}

		void GraphMolecules ()
		{
			foreach (MoleculeBinder binder in validBinders)
			{
				graphedBinders.Add( new BinderAngle( binder, GetMoleculeAngleFromForward( binder.molecule ) ) );
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
