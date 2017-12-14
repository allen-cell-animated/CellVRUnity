using UnityEngine;
using System.Collections;

namespace UtopiaWorx.Helios.Effects
{
[ExecuteInEditMode]
public class FadeBlack : MonoBehaviour 
{
	public float Amount = 0.0f;
	public Material SourceMaterial;
		public Color FadeColor= Color.black;

	[ImageEffectOpaque]
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if(SourceMaterial == null )
		{
			Graphics.Blit(source, destination);
		}
		else
		{
			SourceMaterial.SetFloat("_Amount",Amount);
			SourceMaterial.SetColor("_Color",FadeColor);
			Graphics.Blit(source, destination, SourceMaterial,0);

		}

	}
}

}