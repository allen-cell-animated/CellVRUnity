using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using AICS;
using System.IO;

public class DistributionTest
{
	[Test]
	public void ExponentialDistribution10 ()
	{
		string data = "";
		int n = 0;
		for (int i = 0; i < 1000; i++)
		{
			data += Helpers.SampleExponentialDistribution( 10f ) + "\n";
			n++;
		}
		File.WriteAllText( "/Users/blairl/Desktop/TestExponential10.csv", data );
		Assert.AreEqual( 1000, n );
	}

	[Test]
	public void NormalDistribution50 ()
	{
		string data = "";
		int n = 0;
		for (int i = 0; i < 1000; i++)
		{
			data += Helpers.SampleNormalDistribution( 50f, 10f ) + "\n";
			n++;
		}
		File.WriteAllText( "/Users/blairl/Desktop/TestNormal50.csv", data );
		Assert.AreEqual( 1000, n );
	}
}
