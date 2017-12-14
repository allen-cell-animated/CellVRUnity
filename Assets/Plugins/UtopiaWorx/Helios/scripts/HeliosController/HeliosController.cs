using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace UtopiaWorx.Helios
{
	[HelpURL("http://www.utopiaworx.com/Helios/Helios_Controller.aspx")]
public class HeliosController : MonoBehaviour 
{


	[SerializeField]
	
		public float JumpVelocity = 500.0f;
		public float HorizontalSpeed;
	private Vector3 tempPosition;
		public bool BypassMode = false;
		public Animator TheAnimator;
	UtopiaWorx.Helios.SessionData FrameController;


		public void SetFrameInput(UtopiaWorx.Helios.SessionData FrameInput)
		{
			FrameController = FrameInput;
		}

	void Start () 
	{
		Cursor.visible = false;
		tempPosition = transform.position;
			try
			{
				TheAnimator = gameObject.GetComponent<Animator>();
			}
			catch
			{
				TheAnimator = null;
			}
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
			if(BypassMode == false)
			{
			if(FrameController == null)
			{
		
			float Mod = HorizontalSpeed;
			if(Input.GetKey(KeyCode.LeftShift))
			{
				Mod = HorizontalSpeed + HorizontalSpeed;
			}

			if(Input.GetAxis("Vertical") > 0)
			{
				transform.Translate((Vector3.forward/4) * Mod);
			}
			if(Input.GetAxis("Vertical") < 0)
			{
				transform.Translate((Vector3.back * Mod ) * 0.25f);

			}


			if(Input.GetAxis("Horizontal") < 0)
			{
				transform.Translate((Vector3.left/4) * Mod);
			}

			if(Input.GetAxis("Horizontal") > 0)
			{
				transform.Translate((Vector3.right/4) * Mod);
			}
			tempPosition= transform.position;

			transform.position = tempPosition;

				if(Input.GetKeyDown(KeyCode.Space)== true)
				{
					gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * JumpVelocity);
				}

		
	

		

			}
			else
			{
				transform.position = FrameController.Position;
                    transform.localScale = FrameController.Scale;
				//transform.eulerAngles = FrameController.Rotation;
					transform.rotation = FrameController.Rotation;

					if(TheAnimator != null)
					{
						if(FrameController.AnimationDetails != null)
						{
							foreach(SessionDataAnimationDetails MyDetails in FrameController.AnimationDetails)
							{

								switch(MyDetails.Type)
								{
								case AnimatorControllerParameterType.Bool:
									TheAnimator.SetBool(MyDetails.NameValue,MyDetails.BoolValue);

									break;
								case AnimatorControllerParameterType.Int:
									TheAnimator.SetInteger(MyDetails.NameValue,MyDetails.IntValue);

									break;
								case AnimatorControllerParameterType.Float:
									TheAnimator.SetFloat(MyDetails.NameValue,MyDetails.FloatValue);
									break;
								}
							}
						}
					}
			}
			}
	}
}
}
