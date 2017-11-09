using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MT
{
	public interface IWalkSplines
	{
		GameObject gameObject { get; }

		Transform transform { get; }

		float t { get; set; }

		Microtubule microtubule { get; set; }
	}
}