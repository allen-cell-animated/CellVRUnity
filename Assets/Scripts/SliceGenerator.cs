using UnityEngine;
using System.Collections;

public class SliceGenerator : MonoBehaviour 
{
	public string[] spriteFolderNames = new string[3];
	public string[] spriteStackBaseNames = new string[3]; //  C1-3500000583_100X_20170213_E05_P07_5.ome
	public int spriteNumberDigits = 4; // 0000
	public int slicesPerDimension = 4;
	public float distanceBetweenSlices = 1f;

	Sprite[][] sprites;

	SpriteRenderer _slicePrefab;
	SpriteRenderer slicePrefab
	{
		get
		{
			if (_slicePrefab == null)
			{
				_slicePrefab = (Resources.Load("SlicePrefab") as GameObject).GetComponent<SpriteRenderer>();
			}
			return _slicePrefab;
		}
	}

	void Start () 
	{
		GenerateSlices();
	}

	void GenerateSlices ()
	{
		LoadSprites();

		for (int i = 0; i < sprites.Length; i++) // foreach dimension 
		{
			float depth = (-distanceBetweenSlices * (slicesPerDimension - 1f)) / 2f;
			for (int j = 22; j < sprites[i].Length; j += 2) // foreach image in dimension
			{
				AddSlice(new Vector3(0, 0, depth), sprites[i][j]);
				depth += distanceBetweenSlices;
			}
		}
	}

	void LoadSprites ()
	{
		sprites = new Sprite[3][];

		for (int i = 0; i < spriteStackBaseNames.Length; i++) // foreach dimension
		{
			sprites[i] = new Sprite[slicesPerDimension];
			for (int j = 21; j < slicesPerDimension; j++) // foreach image in dimension
			{
				string path = "Slices/" + spriteFolderNames[i] + "/" + spriteStackBaseNames[i] + FormattedSpriteNumber(j);
				sprites[i][j] = Resources.Load<Sprite>(path);
				Debug.Log(path + " : " + (sprites[i][j] == null));
			}
		}
	}

	string FormattedSpriteNumber (int number)
	{
		string numberString = number.ToString();
		while (numberString.Length < spriteNumberDigits)
		{
			numberString = "0" + numberString;
		}
		return numberString;
	}

	void AddSlice (Vector3 position, Sprite sprite)
	{
		Debug.Log(sprite == null);
		SpriteRenderer spriteRenderer = Instantiate(slicePrefab, transform) as SpriteRenderer;
		spriteRenderer.sprite = sprite;
		spriteRenderer.transform.localPosition = position;
	}
}
