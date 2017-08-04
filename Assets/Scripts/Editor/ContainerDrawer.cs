using UnityEngine;
using UnityEditor;

namespace AICS
{
	[CustomEditor( typeof(Container) )]
	public class ContainerEditor : Editor
	{
		public override void OnInspectorGUI () 
		{
			Container container = (Container)target;

			EditorGUI.BeginChangeCheck();

			container.size = EditorGUILayout.Vector3Field( "Size", container.size );
			container.wallWidth = EditorGUILayout.FloatField( "Wall width", container.wallWidth );

			if (EditorGUI.EndChangeCheck())
			{
				container.SetWalls();
			}
		}
	}
}