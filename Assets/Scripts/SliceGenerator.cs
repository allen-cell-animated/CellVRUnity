using UnityEngine;
using System.Collections;

public class SliceGenerator : MonoBehaviour 
{
	public string slicePrefabName = "SlicePrefab";
	public string[] spriteFolderNames = new string[3];
	public string[] spriteStackBaseNames = new string[3]; //  C1-3500000583_100X_20170213_E05_P07_5.ome
	public int spriteNumberDigits = 4; // 0000
	public int slicesPerDimension = 4;
	public float distanceBetweenSlices = 1f;
	public int startSlice = 12;

	Sprite[][] sprites;

	SpriteRenderer _slicePrefab;
	SpriteRenderer slicePrefab
	{
		get
		{
			if (_slicePrefab == null)
			{
				_slicePrefab = (Resources.Load(slicePrefabName) as GameObject).GetComponent<SpriteRenderer>();
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
			float depth = (-distanceBetweenSlices * ((slicesPerDimension - startSlice) - 1f)) / 2f;
			for (int j = startSlice; j < sprites[i].Length; j++) // foreach image in dimension
			{
				AddSlice(new Vector3(0, 0, depth), sprites[i][j]);
				depth += distanceBetweenSlices;
			}
		}
	}

	void LoadSprites ()
	{
		sprites = new Sprite[spriteFolderNames.Length][];

		for (int i = 0; i < spriteStackBaseNames.Length; i++) // foreach dimension
		{
			sprites[i] = new Sprite[slicesPerDimension];
			for (int j = startSlice; j < slicesPerDimension; j++) // foreach image in dimension
			{
				string path = "Slices/" + spriteFolderNames[i] + "/" + spriteStackBaseNames[i] + FormattedSpriteNumber(j);
				sprites[i][j] = Resources.Load<Sprite>(path);
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
		SpriteRenderer spriteRenderer = Instantiate(slicePrefab, transform) as SpriteRenderer;
		spriteRenderer.sprite = sprite;
		spriteRenderer.transform.localPosition = position;
	}
}
