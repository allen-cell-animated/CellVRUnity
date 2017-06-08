using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class StaticCompoundCollider : MonoBehaviour 
	{
		ICompoundStaticColliderParent _parent;
		ICompoundStaticColliderParent parent
		{
			get {
				if (_parent == null)
				{
					_parent = GetComponentInParent<ICompoundStaticColliderParent>();
				}
				return _parent;
			}
		}

		void OnCollisionEnter (Collision collision)
		{
			if (parent != null)
			{
				parent.Collided( collision );
			}
		}
	}
}
