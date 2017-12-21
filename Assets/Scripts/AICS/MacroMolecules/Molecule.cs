using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.MacroMolecules
{
	public enum MoleculeType
	{
		None,
		All,
		TubulinA,
		TubulinB,
		Microtubule,
		KinesinMotor,
		KinesinHips,
		KinesinNecklinker,
		KinesinCargo,
		KinesinTropomyosin,
		Kinesin,
		ATP,
		ADP,
		Pi,
		ActinMonomer
	}

	// A basic molecule object
	public class Molecule : MonoBehaviour 
	{
		public MoleculeType type;
		public float radius = 1f;
		public GameObject[] componentObjects;
		public Polymer polymer;

		void Start ()
		{
			DoSetup();
		}

		void Update ()
		{
			FinishReset();
		}

		public void DoStep ()
		{
			DoSimulationStep();
		}

		List<T> GetMolecularComponents<T> ()
		{
			List<T> list = new List<T>();
			foreach (GameObject componentObj in componentObjects)
			{
				list.AddRange( componentObj.GetComponents<T>() );
			}
			list.AddRange( GetComponents<T>() );
			return list;
		}

		// --------------------------------------------------------------------------------------------------- Setup

		List<ISetup> _setterUppers;
		List<ISetup> setterUppers
		{
			get
			{
				if (_setterUppers == null)
				{
					_setterUppers = GetMolecularComponents<ISetup>();
				}
				return _setterUppers;
			}
		}

		void DoSetup ()
		{
			foreach (ISetup setterUpper in setterUppers)
			{
				setterUpper.DoSetup();
			}
		}

		// --------------------------------------------------------------------------------------------------- Interactions

		public bool bound
		{
			get
			{
				return GetAnOccupiedBinder() != null;
			}
		}

		public MoleculeBinder GetAnOccupiedBinder ()
		{
			foreach (MoleculeBinder binder in binders)
			{
				if (binder.boundBinder != null)
				{
					return binder;
				}
			}
			return null;
		}

		List<MoleculeBinder> _binders;
		public List<MoleculeBinder> binders
		{
			get
			{
				if (_binders == null)
				{
					_binders = GetMolecularComponents<MoleculeBinder>();
				}
				return _binders;
			}
		}

		public bool IsBoundToMolecule (Molecule other)
		{
			foreach (MoleculeBinder binder in binders)
			{
				if (binder.boundBinder != null && binder.boundBinder.molecule == other)
				{
					return true;
				}
			}
			foreach (MoleculeBinder binder in binders)
			{
				if (binder.boundBinder != null && binder.boundBinder.molecule == other)
				{
					return true;
				}
			}
			return false;
		}

		public List<MoleculeBinder> GetOpenBinders (BindingCriteria criteria)
		{
			List<MoleculeBinder> bs = new List<MoleculeBinder>();
			foreach (MoleculeBinder b in binders)
			{
				if (b.IsAvailableMatch( criteria ))
				{
					bs.Add( b );
				}
			}
			return bs;
		}

		List<BindHandler> _bindHandlers;
		List<BindHandler> bindHandlers
		{
			get
			{
				if (_bindHandlers == null)
				{
					_bindHandlers = GetMolecularComponents<BindHandler>();
				}
				return _bindHandlers;
			}
		}

		void OnEnable ()
		{
			foreach (BindHandler bindHandler in bindHandlers)
			{
				bindHandler.SubscribeToAllBinders( binders );
			}
		}

		// --------------------------------------------------------------------------------------------------- Movement

		public bool canMove
		{
			get
			{
				return !bound && !MolecularEnvironment.Instance.pause;
			}
		}

		List<IValidateMoves> _moveValidators;
		List<IValidateMoves> moveValidators
		{
			get
			{
				if (_moveValidators == null)
				{
					_moveValidators = GetMolecularComponents<IValidateMoves>();
				}
				return _moveValidators;
			}
		}

		List<Leash> _leashes;
		public List<Leash> leashes
		{
			get
			{
				if (_leashes == null)
				{
					_leashes = GetMolecularComponents<Leash>();
				}
				return _leashes;
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

		List<ISimulate> _simulators;
		List<ISimulate> simulators
		{
			get
			{
				if (_simulators == null)
				{
					_simulators = GetMolecularComponents<ISimulate>();
				}
				return _simulators;
			}
		}

		void DoSimulationStep ()
		{
			foreach (ISimulate simulator in simulators)
			{
				simulator.DoSimulationStep();
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

		void FinishReset ()
		{
			if (resetFrames > 0)
			{
				resetFrames--;
			}
		}

		public void Restart ()
		{
			transform.position = startPosition;
			transform.rotation = startRotation;
			resetFrames = 3;
		}
	}
}