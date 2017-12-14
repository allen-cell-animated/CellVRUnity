using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UtopiaWorx.Helios
{
[System.Serializable]
public class ManagedObject
{
		[SerializeField]
		public GameObject TargetGameObject;
		[SerializeField]
		public bool RecordArmed = false;
		[SerializeField]
		public bool PlaybackArmed = false;
		[SerializeField]
		public string SessionName;
		[SerializeField]
		public bool IsCollapsed = false;
		[SerializeField]
		public int Mode = 0;


}
}