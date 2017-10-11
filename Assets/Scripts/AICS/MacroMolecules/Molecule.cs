using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;
using System.ComponentModel.Design;

namespace AICS.MacroMolecules
{
	public enum MoleculeType
	{
		All,
		None,
		TubulinA,
		TubulinB,
		Microtubule,
		KinesinMotor,
		KinesinHips,
		KinesinNecklinker,
		KinesinCargo,
		KinesinTropomyosin,
		Kinesin
	}

	// A basic molecule object
	public class Molecule : MonoBehaviour 
	{
		public MoleculeType type;
		public float radius = 1f;
		public bool bound;

		// --------------------------------------------------------------------------------------------------- Interactions

		IBind[] _binders;
		IBind[] binders
		{
			get
			{
				if (_binders == null)
				{
					_binders = GetComponents<IBind>();
				}
				return _binders;
			}
		}

		public bool BoundToType (MoleculeType _type)
		{
			foreach (IBind binder in binders)
			{
				if (binder.typeToBind == _type && binder.boundMolecule != null)
				{
					return true;
				}
			}
			return false;
		}

		// --------------------------------------------------------------------------------------------------- Movement

		public bool canMove
		{
			get
			{
				return !bound && !MolecularEnvironment.Instance.pause;
			}
		}

		IValidateMoves[] _moveValidators;
		IValidateMoves[] moveValidators
		{
			get
			{
				if (_moveValidators == null)
				{
					_moveValidators = GetComponents<IValidateMoves>();
				}
				return _moveValidators;
			}
		}

		public bool MoveIfValid (Vector3 moveStep)
		{
			if (MoveIsValid( moveStep ))
			{
				IncrementPosition( moveStep );
				return true;
			}
			return false;
		}

		bool MoveIsValid (Vector3 moveStep)
		{
			if (!canMove)
			{
				return false;
			}
			foreach (IValidateMoves moveValidator in moveValidators)
			{
				if (!moveValidator.MoveIsValid( transform.position + moveStep, radius ))
				{
					return false;
				}
			}
			return true;
		}

		void IncrementPosition (Vector3 moveStep)
		{
			SetPosition( transform.position + moveStep );
		}

		public void SetPosition (Vector3 newPosition)
		{
			if (canMove)
			{
				transform.position = (resetFrames > 0) ? startPosition : newPosition;
			}
		}

		// --------------------------------------------------------------------------------------------------- Simulation

		ISimulate[] _simulators;
		ISimulate[] simulators
		{
			get
			{
				if (_simulators == null)
				{
					_simulators = GetComponents<ISimulate>();
				}
				return _simulators;
			}
		}

		// --------------------------------------------------------------------------------------------------- Reset

		Vector3 startPosition;
		Quaternion startRotation;
		protected int resetFrames;

		void Awake ()
		{
			startPosition = transform.position;
			startRotation = transform.rotation;
		}

		void Update ()
		{
			FinishReset();
		}

		protected virtual void FinishReset ()
		{
			if (resetFrames > 0)
			{
				resetFrames--;
			}
		}

		public void Reset ()
		{
			transform.position = startPosition;
			transform.rotation = startRotation;
			resetFrames = 3;
		}
	}
}