using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using AICS;

public class MatrixTest 
{
	[Test]
	public void Determinant2x2() 
	{
		ArbitraryMatrix matrix = new ArbitraryMatrix( 2, 2 );
		matrix[0] = new float[]{4, 7};
		matrix[1] = new float[]{2, 6};

		Assert.AreEqual( 10, matrix.determinant );
	}

	[Test]
	public void Determinant3x3() 
	{
		ArbitraryMatrix matrix = new ArbitraryMatrix( 3, 3 );
		matrix[0] = new float[]{2, 4, 3};
		matrix[1] = new float[]{5,-6, 1};
		matrix[2] = new float[]{7, 5, 4};

		Assert.AreEqual( 91, matrix.determinant );
	}

	[Test]
	public void Determinant4x4() 
	{
		ArbitraryMatrix matrix = new ArbitraryMatrix( 4, 4 );
		matrix[0] = new float[]{2, 4, 3, 6};
		matrix[1] = new float[]{5,-6, 1, 4};
		matrix[2] = new float[]{7, 5, 4, 0};
		matrix[3] = new float[]{2,14, 2, 5};

		Assert.AreEqual( 1683f, matrix.determinant );
	}

	[Test]
	public void Determinant6x6() 
	{
		ArbitraryMatrix matrix = new ArbitraryMatrix( 6, 6 );
		matrix[0] = new float[]{2, 4, 3, 6, 8, 10};
		matrix[1] = new float[]{5,-6, 1, 4, 3, 3};
		matrix[2] = new float[]{7, 5, 4, 0, 0, 12};
		matrix[3] = new float[]{2, 14,2, 5, 6, 11};
		matrix[4] = new float[]{3,-7,-1,-4, 6, 7};
		matrix[5] = new float[]{4, 6, 4, 6, 4, 1};

		Assert.AreEqual( -181682, matrix.determinant );
	}

	[Test]
	public void Cofactor3x3() 
	{
		ArbitraryMatrix matrix = new ArbitraryMatrix( 3, 3 );
		matrix[0] = new float[]{2, 4, 3};
		matrix[1] = new float[]{5,-6, 1};
		matrix[2] = new float[]{7, 5, 4};

		ArbitraryMatrix cofactor = new ArbitraryMatrix( 3, 3 );
		cofactor[0] = new float[]{-29,-13, 67};
		cofactor[1] = new float[]{ -1,-13, 18};
		cofactor[2] = new float[]{ 22, 13,-32};

		ArbitraryMatrix test = matrix.coFactor;

		Assert.AreEqual( cofactor[0][0], test[0][0] );
		Assert.AreEqual( cofactor[0][1], test[0][1] );
		Assert.AreEqual( cofactor[0][2], test[0][2] );
		Assert.AreEqual( cofactor[1][0], test[1][0] );
		Assert.AreEqual( cofactor[1][1], test[1][1] );
		Assert.AreEqual( cofactor[1][2], test[1][2] );
		Assert.AreEqual( cofactor[2][0], test[2][0] );
		Assert.AreEqual( cofactor[2][1], test[2][1] );
		Assert.AreEqual( cofactor[2][2], test[2][2] );
	}

	[Test]
	public void Transpose3x3() 
	{
		ArbitraryMatrix matrix = new ArbitraryMatrix( 3, 3 );
		matrix[0] = new float[]{2, 4, 3};
		matrix[1] = new float[]{5,-6, 1};
		matrix[2] = new float[]{7, 5, 4};

		ArbitraryMatrix transpose = new ArbitraryMatrix( 3, 3 );
		transpose[0] = new float[]{2, 5, 7};
		transpose[1] = new float[]{4,-6, 5};
		transpose[2] = new float[]{3, 1, 4};

		ArbitraryMatrix test = matrix.transpose;

		Assert.AreEqual( transpose[0][0], test[0][0] );
		Assert.AreEqual( transpose[0][1], test[0][1] );
		Assert.AreEqual( transpose[0][2], test[0][2] );
		Assert.AreEqual( transpose[1][0], test[1][0] );
		Assert.AreEqual( transpose[1][1], test[1][1] );
		Assert.AreEqual( transpose[1][2], test[1][2] );
		Assert.AreEqual( transpose[2][0], test[2][0] );
		Assert.AreEqual( transpose[2][1], test[2][1] );
		Assert.AreEqual( transpose[2][2], test[2][2] );
	}

	[Test]
	public void Inverse3x3() 
	{
		ArbitraryMatrix matrix = new ArbitraryMatrix( 3, 3 );
		matrix[0] = new float[]{2, 4, 3};
		matrix[1] = new float[]{5,-6, 1};
		matrix[2] = new float[]{7, 5, 4};

		ArbitraryMatrix identity = new ArbitraryMatrix( 3, 3 );
		identity[0] = new float[]{1, 0, 0};
		identity[1] = new float[]{0, 1, 0};
		identity[2] = new float[]{0, 0, 1};

		ArbitraryMatrix test = matrix * matrix.inverse;

		Assert.AreEqual( identity[0][0], Mathf.Round( 1000f * test[0][0] ) / 1000f );
		Assert.AreEqual( identity[0][1], Mathf.Round( 1000f * test[0][1] ) / 1000f );
		Assert.AreEqual( identity[0][2], Mathf.Round( 1000f * test[0][2] ) / 1000f );
		Assert.AreEqual( identity[1][0], Mathf.Round( 1000f * test[1][0] ) / 1000f );
		Assert.AreEqual( identity[1][1], Mathf.Round( 1000f * test[1][1] ) / 1000f );
		Assert.AreEqual( identity[1][2], Mathf.Round( 1000f * test[1][2] ) / 1000f );
		Assert.AreEqual( identity[2][0], Mathf.Round( 1000f * test[2][0] ) / 1000f );
		Assert.AreEqual( identity[2][1], Mathf.Round( 1000f * test[2][1] ) / 1000f );
		Assert.AreEqual( identity[2][2], Mathf.Round( 1000f * test[2][2] ) / 1000f );
	}

	[Test]
	public void SolveMatrixEquation() 
	{
		ArbitraryMatrix matrix = new ArbitraryMatrix( 3, 3 );
		matrix[0] = new float[]{5, 8, -4};
		matrix[1] = new float[]{6, 9, -5};
		matrix[2] = new float[]{4, 7, -2};

		float[] vector = new float[3]{2, -3, 1};
		float[] solution = new float[3]{-18, -20, -15};

		float[] test = matrix * vector;

		Assert.AreEqual( solution[0], test[0] );
		Assert.AreEqual( solution[1], test[1] );
		Assert.AreEqual( solution[2], test[2] );
	}
}
