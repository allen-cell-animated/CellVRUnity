using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Necklinker
{
	public class Necklinker : MonoBehaviour 
	{
		public bool startDocked;
		public bool snapping;
		public bool bound;

		float lastTime = -10f;
		bool resetting;

		Link[] _links;
		public Link[] links
		{
			get {
				if (_links == null)
				{
					_links = GetComponentsInChildren<Link>();
				}
				return _links;
			}
		}

		Motor _motor;
		public Motor motor
		{
			get {
				if (_motor == null)
				{
					_motor = GetComponentInParent<Motor>();
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
				links[0].StartSnapping( 0 );
			}
		}

		public void FinishSnapping ()
		{
			snapping = false;
		}

		public void StopSnapping ()
		{
			foreach (Link link in links)
			{
				link.StopSnapping();
			}
			snapping = false;
		}

		public void Release ()
		{
			foreach (Link link in links)
			{
				link.Release();
			}
			snapping = bound = resetting = false;
		}

		public void Reset ()
		{
			foreach (Link link in links)
			{
				link.ResetPosition();
			}
			snapping = bound = false;
			lastTime = Time.time;
			resetting = true;
		}
	}
}
