using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class VertexParticles : MonoBehaviour 
{
	public Color color;
	public float size = 10f;
	public bool gradientShading;

	Mesh _mesh;
	Mesh mesh
	{
		get
		{
			if (_mesh == null)
			{
				_mesh = GetComponent<MeshFilter>().sharedMesh;
			}
			return _mesh;
		}
	}

	Vector3 center;
	float maxRadius;

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

		if (mesh == null)
		{
			Debug.LogError(name + " has no mesh to emit particles from!");
			return;
		}

		ParticleSystem.EmitParams emitParameters = GetEmitParameters();
		if (gradientShading)
		{
			SetCenter();
			SetMaxRadius();
		}

		foreach (Vector3 vertex in mesh.vertices)
		{
			emitParameters.position = vertex;
			if (gradientShading) { emitParameters.startColor = ColorForVertexRadius(vertex); }
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

	Color ColorForVertexRadius (Vector3 vertex)
	{
		float normalizedRadius = VertexRadius(vertex) / maxRadius;
		return new Color(Remap(color.r, normalizedRadius), Remap(color.g, normalizedRadius), Remap(color.b, normalizedRadius), color.a);
	}

	float Remap (float colorComponent, float normalizedVertexRadius)
	{
//		return colorComponent + (1f - normalizedVertexRadius) * (1f - colorComponent); // white center
		return colorComponent * normalizedVertexRadius; // black center
	}

	float VertexRadius (Vector3 vertex)
	{
		return Vector3.Distance(center, vertex);
	}

	void SetMaxRadius ()
	{
		maxRadius = 0;
		float d;
		foreach (Vector3 vertex in mesh.vertices)
		{
			d = VertexRadius(vertex);
			if (d > maxRadius)
			{
				maxRadius = d;
			}
		}
	}

	void SetCenter ()
	{
		center = Vector3.zero;
		foreach (Vector3 vertex in mesh.vertices)
		{
			center += vertex;
		}
		center /= mesh.vertices.Length;
	}
}
