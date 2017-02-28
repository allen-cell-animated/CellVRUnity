using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class GlobalShaderVariables : MonoBehaviour
{
//	[SerializeField]
//	private Texture2D noiseOffsetTexture;

//	private void OnPreRender()
//	{
//		Shader.SetGlobalVector("_CamPos", this.transform.position);
//		Shader.SetGlobalVector("_CamRight", this.transform.right);
//		Shader.SetGlobalVector("_CamUp", this.transform.up);
//		Shader.SetGlobalVector("_CamForward", this.transform.forward);
//
////		Shader.SetGlobalFloat("_AspectRatio", (float)Screen.width / (float)Screen.height);
////		Shader.SetGlobalFloat("_FieldOfView", Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * 2f);
////		Shader.SetGlobalTexture("_NoiseOffsets", this.noiseOffsetTexture);
//	}

	Camera _thisCamera;
	Camera thisCamera
	{
		get 
		{
			if (_thisCamera == null)
			{
				_thisCamera = GetComponent<Camera>();
			}
			return _thisCamera;
		}
	}

	private void OnPreRender()
	{
		Shader.SetGlobalMatrix("_ViewMatrix", thisCamera.projectionMatrix * thisCamera.worldToCameraMatrix);
		Shader.SetGlobalVector("_CameraPosition", transform.position);
	}
}

