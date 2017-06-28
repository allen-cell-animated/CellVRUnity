using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class NecklinkerTest : MonoBehaviour 
	{
		public bool startDocked;
		public bool snapping;
		public bool bound;

		float lastTime = -10f;
		bool resetting;

		LinkTest[] _links;
		public LinkTest[] links
		{
			get {
				if (_links == null)
				{
					_links = GetComponentsInChildren<LinkTest>();
				}
				return _links;
			}
		}

		MotorTest _motor;
		public MotorTest motor
		{
			get {
				if (_motor == null)
				{
					_motor = GetComponentInParent<MotorTest>();
				}
				return _motor;
			}
		}

		public void SetDockedPositions (Vector3[] dockedPositions)
		{
			for (int i = 0; i < links.Length; i++)
			{
				links[i].dockedPosition = dockedPositions[i];
			}
		}

		void Update ()
		{
//			if (!snapping)
//			{
//				if (bound && Time.time - lastTime > 1f)
//				{
//					Release();
//					lastTime = Time.time;
//				}
//				else if (Time.time - lastTime > 10f)
//				{
//					StartSnapping();
//				}
//			}
			if (resetting && Time.time - lastTime > 1f)
			{
				Release();
			}
		}

		public void StartSnapping ()
		{
			if (resetting)
			{
				Release();
			}
			if (!snapping)
			{
				snapping = bound = true;
				resetting = false;
				links[0].StartSnapping();
			}
		}

		public void FinishSnapping ()
		{
			snapping = false;
//			lastTime = Time.time;
		}

		public void StopSnapping ()
		{
			foreach (LinkTest link in links)
			{
				link.StopSnapping();
			}
			snapping = false;
		}

		public void Release ()
		{
			foreach (LinkTest link in links)
			{
				link.Release();
			}
			snapping = bound = resetting = false;
		}

		public void Reset ()
		{
			foreach (LinkTest link in links)
			{
				link.ResetPosition();
			}
			snapping = bound = false;
			lastTime = Time.time;
			resetting = true;
		}

		public void RetrySnapping ()
		{
			Debug.Log( "Retry" );
			Release();
			Invoke( "StartSnapping", 0.2f );
		}
	}
}
