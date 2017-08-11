using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AICS.AnimatedKinesin
{
	public class CreateKineticRates 
	{
		[MenuItem("Assets/Create/KinesinKineticRates")]
		public static void CreateKinesinKineticRates ()
		{
			KineticRates asset = ScriptableObject.CreateInstance<KineticRates>();
			asset.SetKinesinDefaults();

			AssetDatabase.CreateAsset(asset, "Assets/Data/KineticRates/newKinesinKineticRates.asset");
			AssetDatabase.SaveAssets();

			EditorUtility.FocusProjectWindow();

			Selection.activeObject = asset;
		}
	}
}