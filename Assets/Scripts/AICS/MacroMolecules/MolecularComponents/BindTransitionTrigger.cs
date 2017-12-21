using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class BindTransitionTrigger : BindHandler
	{
		public MoleculeBinder binder;
		public StateMachine stateMachineToTrigger;
		public StateTransitionID[] transitionsToTriggerOnBind;
		public StateTransitionID[] transitionsToTriggerOnRelease;

		void Awake ()
		{
			subscribeToAll = false;
			SubscribeToBinder( binder );
		}

		protected override void OnBind (MoleculeBinder binder)
		{
			foreach (StateTransitionID id in transitionsToTriggerOnBind)
			{
				stateMachineToTrigger.ForceTransition( id );
			}
		}

		protected override void OnRelease (MoleculeBinder binder)
		{
			foreach (StateTransitionID id in transitionsToTriggerOnRelease)
			{
				stateMachineToTrigger.ForceTransition( id );
			}
		}
	}
}
