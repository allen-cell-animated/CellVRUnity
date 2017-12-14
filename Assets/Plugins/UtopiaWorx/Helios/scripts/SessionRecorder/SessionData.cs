using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UtopiaWorx.Helios
{
	[System.Serializable]
	public class SessionDataAnimationDetails : System.Object
	{

		[SerializeField]
		public AnimatorControllerParameterType Type;

		[SerializeField]
		public string NameValue;

		[SerializeField]
		public int IntValue;

		[SerializeField]	
		public float FloatValue;

		[SerializeField]
		public bool BoolValue;

	}

	[System.Serializable]
	public class SessionData : System.Object 
	{
		[SerializeField]
		public Vector3 Position;

//		[SerializeField]
//		public Vector3 Rotation;

		[SerializeField]
		public Quaternion Rotation;

        [SerializeField]
        public Vector3 Scale;

//		[SerializeField]
//		public float GameTime = 0.0f;
		[SerializeField]
		public int FrameID =0;
		[SerializeField]
		public bool W = false;
		[SerializeField]
		public bool A = false;
		[SerializeField]
		public bool S = false;
		[SerializeField]
		public bool D = false;
		[SerializeField]
		public bool Shift = false;
		[SerializeField]
		public bool Space = false;
		[SerializeField]
		public float mouseX = 0.0f;
		[SerializeField]
		public float mouseY = 0.0f;

		[SerializeField]
		public List<SessionDataAnimationDetails> AnimationDetails;

		public SessionData()
		{
			
		}
		[SerializeField]
		public int AnimationState = 0;



	}

}
