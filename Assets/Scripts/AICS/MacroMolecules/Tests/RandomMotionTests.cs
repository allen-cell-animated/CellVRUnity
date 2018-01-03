using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using AICS.MacroMolecules;

public class RandomMotionTests
{
	[UnityTest]
	public IEnumerator SingleMoleculeContainer ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		MolecularEnvironment.Instance.size = 10f * Vector3.one;
		GameObject molecule = GameObject.Instantiate( Resources.Load( "Tests/RandomMotion/SingleMoleculeContainer" ) as GameObject ) as GameObject;

		int n = 500;
		while (n > 0)
		{
			Assert.IsTrue( MolecularEnvironment.Instance.PointIsInBounds( molecule.transform.position ) );

			yield return new WaitForEndOfFrame();

			n--;
		}

		yield return null;
	}

	[UnityTest]
	public IEnumerator BranchedContainer ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		MolecularEnvironment.Instance.size = 20f * Vector3.one;
		GameObject polymer = GameObject.Instantiate( Resources.Load( "Tests/RandomMotion/BranchedMoleculeContainer" ) as GameObject ) as GameObject;
		GameObject molecule = polymer.transform.GetChild( 0 ).GetChild( 0 ).gameObject;

		int n = 500;
		while (n > 0)
		{
			Assert.IsTrue( MolecularEnvironment.Instance.PointIsInBounds( molecule.transform.position ) );

			yield return new WaitForEndOfFrame();

			n--;
		}

		yield return null;
	}
}