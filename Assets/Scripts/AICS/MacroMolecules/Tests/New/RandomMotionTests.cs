using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using AICS.MacroMolecules;

public class RandomMotionTests
{
	[UnityTest]
	public IEnumerator SingleMoleculeContainerTest ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		MolecularEnvironment.Instance.size = 10f * Vector3.one;
		GameObject molecule = GameObject.Instantiate( Resources.Load( "Tests/RandomMotionMolecule" ) as GameObject ) as GameObject;

		int n = 500;
		while (n > 0)
		{
			Assert.IsTrue( MolecularEnvironment.Instance.PointIsInBounds( molecule.transform.position ) );

			yield return new WaitForFixedUpdate();

			n--;
		}

		yield return null;
	}

	[UnityTest]
	public IEnumerator AssemblyMoleculeContainerTest ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		MolecularEnvironment.Instance.size = 10f * Vector3.one;
		GameObject assembly = GameObject.Instantiate( Resources.Load( "Tests/RandomMotionAssemblyMolecule" ) as GameObject ) as GameObject;
		GameObject molecule = assembly.transform.GetChild( 0 ).GetChild( 0 ).gameObject;

		int n = 500;
		while (n > 0)
		{
			Assert.IsTrue( MolecularEnvironment.Instance.PointIsInBounds( molecule.transform.position ) );

			yield return new WaitForFixedUpdate();

			n--;
		}

		yield return null;
	}
}