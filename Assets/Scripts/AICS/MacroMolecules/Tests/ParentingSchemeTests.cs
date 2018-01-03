using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using AICS.MacroMolecules;

public class ParentingSchemeTests
{
	[UnityTest]
	public IEnumerator BranchesToRoot1 ()
	{
		int[] branches = new int[]{2, 1, 0, 1, 2, 3, 1, 2, 3};

		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		Polymer polymer = (GameObject.Instantiate( Resources.Load( "Tests/ParentingScheme/BranchesToRoot1" ) as GameObject ) as GameObject).GetComponent<Polymer>();

		for (int i = 0; i < polymer.monomers.Count; i++)
		{
			int n, min = int.MaxValue;
			foreach (Leash leash in polymer.monomers[i].leashes)
			{
				n = leash.GetMinBranchesToMolecule( polymer.rootMonomer );
				if (n < min)
				{
					min = n;
				}
			}
			Assert.IsTrue( min == branches[i] );
		}

		yield return null;
	}

	[UnityTest]
	public IEnumerator BranchesToRoot2 ()
	{
		int[] branches = new int[]{10, 9, 8, 7, 8, 6, 5, 6, 4, 5, 5, 3, 4, 2, 1, 2, 2, 0, 1, 2, 3, 3, 4, 4, 5};

		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		Polymer polymer = (GameObject.Instantiate( Resources.Load( "Tests/ParentingScheme/BranchesToRoot2" ) as GameObject ) as GameObject).GetComponent<Polymer>();

		for (int i = 0; i < polymer.monomers.Count; i++)
		{
			int n, min = int.MaxValue;
			foreach (Leash leash in polymer.monomers[i].leashes)
			{
				n = leash.GetMinBranchesToMolecule( polymer.rootMonomer );
				if (n < min)
				{
					min = n;
				}
			}
			Assert.IsTrue( min == branches[i] );
		}

		yield return null;
	}

	[UnityTest]
	public IEnumerator BindRelease1 ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		Polymer polymer = (GameObject.Instantiate( Resources.Load( "Tests/ParentingScheme/BindRelease1" ) as GameObject ) as GameObject).GetComponent<Polymer>();

		FinderConditional finder = GameObject.FindObjectOfType<FinderConditional>();
		MoleculeBinder binder = GameObject.FindObjectOfType<MoleculeBinder>();

		if (finder.Pass())
		{
			binder.Bind();
		}

		Assert.IsTrue( binder.transform.parent == polymer.transform );

		yield return new WaitForSeconds( 0.1f );

		binder.Release();

		Assert.IsTrue( polymer.rootMonomer.transform.parent == polymer.transform );

		yield return null;
	}

	[UnityTest]
	public IEnumerator BindRelease2 ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		Polymer polymer = (GameObject.Instantiate( Resources.Load( "Tests/ParentingScheme/BindRelease2" ) as GameObject ) as GameObject).GetComponent<Polymer>();

		FinderConditional finder = GameObject.FindObjectOfType<FinderConditional>();
		MoleculeBinder binder = GameObject.FindObjectOfType<MoleculeBinder>();

		if (finder.Pass())
		{
			binder.Bind();
		}

		Assert.IsTrue( binder.transform.parent == polymer.transform );

		yield return new WaitForSeconds( 0.1f );

		binder.Release();

		Assert.IsTrue( polymer.rootMonomer.transform.parent == polymer.transform );

		yield return null;
	}

	[UnityTest]
	public IEnumerator BindRelease3 ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		Polymer polymer = (GameObject.Instantiate( Resources.Load( "Tests/ParentingScheme/BindRelease3" ) as GameObject ) as GameObject).GetComponent<Polymer>();

		FinderConditional finder = GameObject.FindObjectOfType<FinderConditional>();
		MoleculeBinder binder = GameObject.FindObjectOfType<MoleculeBinder>();

		if (finder.Pass())
		{
			binder.Bind();
		}

		Assert.IsTrue( binder.transform.parent == polymer.transform );

		yield return new WaitForSeconds( 0.1f );

		binder.Release();

		Assert.IsTrue( polymer.rootMonomer.transform.parent == polymer.transform );

		yield return null;
	}

	[UnityTest]
	public IEnumerator DoubleBind1 ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		MolecularEnvironment.Instance.size = 10f * Vector3.one;

		GameObject.Instantiate( Resources.Load( "Tests/ParentingScheme/StaticMolecule" ) as GameObject );
		(GameObject.Instantiate( Resources.Load( "Tests/ParentingScheme/StaticMolecule" ) as GameObject ) as GameObject).transform.position = new Vector3( 0.5f, -1f, 0 );
		Polymer polymer = (GameObject.Instantiate( Resources.Load( "Tests/ParentingScheme/DoubleBind1" ) as GameObject ) as GameObject).GetComponent<Polymer>();
		GameObject molecule1 = polymer.transform.GetChild( 0 ).GetChild( 0 ).gameObject;
		GameObject molecule2 = polymer.transform.GetChild( 0 ).GetChild( 1 ).gameObject;

		yield return new WaitForSeconds( 1.1f );

		//molecule1 is bound
		Assert.IsTrue( molecule1.transform.parent == polymer.transform );
		Assert.IsTrue( polymer.rootMonomer.transform.parent == molecule1.transform );
		Assert.IsTrue( molecule2.transform.parent == polymer.rootMonomer.transform );

		yield return new WaitForSeconds( 1f );

		//both molecules are bound
		Assert.IsTrue( molecule1.transform.parent == polymer.transform );
		Assert.IsTrue( polymer.rootMonomer.transform.parent == molecule1.transform );
		Assert.IsTrue( molecule2.transform.parent == polymer.transform );

		yield return new WaitForSeconds( 1f );

		//molecule1 is released leaving only molecule2 bound
		Assert.IsTrue( molecule1.transform.parent == polymer.rootMonomer.transform );
		Assert.IsTrue( polymer.rootMonomer.transform.parent == molecule2.transform );
		Assert.IsTrue( molecule2.transform.parent == polymer.transform );

		yield return new WaitForSeconds( 1f );

		//molecule2 is released so now it's free
		Assert.IsTrue( molecule1.transform.parent == polymer.rootMonomer.transform );
		Assert.IsTrue( polymer.rootMonomer.transform.parent == polymer.transform );
		Assert.IsTrue( molecule2.transform.parent == polymer.rootMonomer.transform );

		yield return null;
	}

	[UnityTest]
	public IEnumerator DoubleBind2 ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		MolecularEnvironment.Instance.size = 40f * Vector3.one;

		GameObject.Instantiate( Resources.Load( "Tests/ParentingScheme/StaticMolecule" ) as GameObject );
		(GameObject.Instantiate( Resources.Load( "Tests/ParentingScheme/StaticMolecule" ) as GameObject ) as GameObject).transform.position = new Vector3( 0.5f, -1f, 0 );
		Polymer polymer = (GameObject.Instantiate( Resources.Load( "Tests/ParentingScheme/DoubleBind2" ) as GameObject ) as GameObject).GetComponent<Polymer>();
		GameObject molecule8 = polymer.transform.GetChild( 0 ).GetChild( 2 ).GetChild( 0 ).GetChild( 0 ).gameObject;
		GameObject molecule2 = polymer.transform.GetChild( 0 ).GetChild( 0 ).GetChild( 0 ).gameObject;

		yield return new WaitForSeconds( 5.1f );

		//molecule8 is bound
		Assert.IsTrue( molecule8.transform.parent == polymer.transform );
		Debug.Log(polymer.rootMonomer == null);
		Assert.IsTrue( polymer.rootMonomer.transform.parent.parent.parent == molecule8.transform );
		Assert.IsTrue( molecule2.transform.parent.parent == polymer.rootMonomer.transform );

		yield return new WaitForSeconds( 5f );

		//both molecules are bound
		Assert.IsTrue( molecule8.transform.parent == polymer.transform );
		Assert.IsTrue( polymer.rootMonomer.transform.parent.parent == molecule2.transform ); //Need to fix
		Assert.IsTrue( molecule2.transform.parent == polymer.transform );

		yield return new WaitForSeconds( 5f );

		//molecule8 is released leaving only molecule2 bound
		Assert.IsTrue( molecule8.transform.parent.parent.parent == polymer.rootMonomer.transform );
		Assert.IsTrue( polymer.rootMonomer.transform.parent.parent == molecule2.transform );
		Assert.IsTrue( molecule2.transform.parent == polymer.transform );

		yield return new WaitForSeconds( 5f );

		//molecule2 is released so now it's free
		Assert.IsTrue( molecule8.transform.parent.parent.parent == polymer.rootMonomer.transform );
		Assert.IsTrue( polymer.rootMonomer.transform.parent == polymer.transform );
		Assert.IsTrue( molecule2.transform.parent.parent == polymer.rootMonomer.transform );

		yield return new WaitForSeconds( 5f );

		yield return null;
	}
}
