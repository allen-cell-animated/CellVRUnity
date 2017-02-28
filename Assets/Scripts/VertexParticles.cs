using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class VertexParticles : MonoBehaviour 
{
	public Color color;
	public float size = 10f;

	ParticleSystem _emitter;
	ParticleSystem emitter
	{
		get
		{
			if (_emitter == null)
			{
				CreateEmitter();
			}
			return _emitter;
		}
	}

	void CreateEmitter ()
	{
		GameObject prefab = Resources.Load("ParticleSystemPrefab") as GameObject;
		if (prefab == null)
		{
			Debug.LogError("Missing ParticleSystemPrefab in Resources folder!");
			return;
		}
		_emitter = (Instantiate(prefab, transform) as GameObject).GetComponent<ParticleSystem>();
		_emitter.transform.localPosition = Vector3.zero;
		_emitter.transform.localRotation = Quaternion.identity;
	}

	void Start () 
	{
		EmitParticles();
	}

	public void EmitParticles ()
	{
		if (emitter == null)
		{
			Debug.LogError("Couldn't initialize particle emitter, does ParticleSystemPrefab in Resources have a ParticleSystem component?");
			return;
		}

		ParticleSystem.EmitParams emitParameters = GetEmitParameters();
		Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

		if (mesh == null)
		{
			Debug.LogError(name + " has no mesh to emit particles from!");
			return;
		}

		foreach (Vector3 vertex in mesh.vertices)
		{
			emitParameters.position = vertex;
			emitter.Emit(emitParameters, 1);
		}
	}

	ParticleSystem.EmitParams GetEmitParameters ()
	{
		ParticleSystem.EmitParams emitParameters = new ParticleSystem.EmitParams();
		emitParameters.velocity = Vector3.zero;
		emitParameters.startLifetime = Mathf.Infinity;
		emitParameters.angularVelocity = 0;
		emitParameters.startColor = color;
		emitParameters.startSize = size;
		return emitParameters;
	}
}
