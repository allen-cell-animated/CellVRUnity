using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class VertexSprites : MonoBehaviour 
{
	public Color color;
	public float size = 1f;

	GameObject _spritePrefab;
	GameObject spritePrefab
	{
		get
		{
			if (_spritePrefab == null)
			{
				_spritePrefab = Resources.Load("SpritePrefab") as GameObject;
				_spritePrefab.transform.localScale = size * Vector3.one;
				SpriteRenderer spriteRenderer = _spritePrefab.GetComponent<SpriteRenderer>();
				if (spriteRenderer != null)
				{
					spriteRenderer.color = color;
				}
			}
			return _spritePrefab;
		}
	}

	void Start () 
	{
		MakeSprites();
	}

	public void MakeSprites ()
	{
		Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

		if (mesh == null)
		{
			Debug.LogError(name + " has no mesh to emit particles from!");
			return;
		}

		foreach (Vector3 vertex in mesh.vertices)
		{
			AddSprite(vertex);
		}
	}

	void AddSprite (Vector3 position)
	{
		GameObject sprite = Instantiate(spritePrefab, transform) as GameObject;
		sprite.transform.localPosition = position;
		sprite.transform.LookAt(Camera.main.transform.position);
	}
}
