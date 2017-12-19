using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using AICS.MacroMolecules;

public class AssemblyParentingSchemeTests
{
	[UnityTest]
	public IEnumerator BranchesToRootTest1 ()
	{
		int[] branches = new int[]{2, 1, 0, 1, 2, 3, 1, 2, 3};

		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		AssemblyMolecule assembly = (GameObject.Instantiate( Resources.Load( "Tests/BranchedAssemblyMolecule1" ) as GameObject ) as GameObject).GetComponent<AssemblyMolecule>();

		for (int i = 0; i < assembly.componentMolecules.Count; i++)
		{
			int n, min = int.MaxValue;
			foreach (Leash leash in assembly.componentMolecules[i].leashes)
			{
				n = leash.GetMinBranchesToComponent( assembly.rootComponent );
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
	public IEnumerator BranchesToRootTest2 ()
	{
		int[] branches = new int[]{10, 9, 8, 7, 8, 6, 5, 6, 4, 5, 5, 3, 4, 2, 1, 2, 2, 0, 1, 2, 3, 3, 4, 4, 5};

		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		AssemblyMolecule assembly = (GameObject.Instantiate( Resources.Load( "Tests/BranchedAssemblyMolecule2" ) as GameObject ) as GameObject).GetComponent<AssemblyMolecule>();

		for (int i = 0; i < assembly.componentMolecules.Count; i++)
		{
			int n, min = int.MaxValue;
			foreach (Leash leash in assembly.componentMolecules[i].leashes)
			{
				n = leash.GetMinBranchesToComponent( assembly.rootComponent );
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
	public IEnumerator BindReleaseTest1 ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		AssemblyMolecule assembly = (GameObject.Instantiate( Resources.Load( "Tests/SpokeAssemblyMolecule" ) as GameObject ) as GameObject).GetComponent<AssemblyMolecule>();

		FinderConditional finder = GameObject.FindObjectOfType<FinderConditional>();
		MoleculeBinder binder = GameObject.FindObjectOfType<MoleculeBinder>();

		if (finder.Pass())
		{
			binder.Bind();
		}

		Assert.IsTrue( binder.transform.parent == assembly.transform );

		yield return new WaitForSeconds( 0.1f );

		binder.Release();

		Assert.IsTrue( assembly.rootComponent.transform.parent == assembly.transform );

		yield return null;
	}

	[UnityTest]
	public IEnumerator BindReleaseTest2 ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		AssemblyMolecule assembly = (GameObject.Instantiate( Resources.Load( "Tests/BranchedAssemblyMolecule3" ) as GameObject ) as GameObject).GetComponent<AssemblyMolecule>();

		FinderConditional finder = GameObject.FindObjectOfType<FinderConditional>();
		MoleculeBinder binder = GameObject.FindObjectOfType<MoleculeBinder>();

		if (finder.Pass())
		{
			binder.Bind();
		}

		Assert.IsTrue( binder.transform.parent == assembly.transform );

		yield return new WaitForSeconds( 0.1f );

		binder.Release();

		Assert.IsTrue( assembly.rootComponent.transform.parent == assembly.transform );

		yield return null;
	}

	[UnityTest]
	public IEnumerator BindReleaseTest3 ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		AssemblyMolecule assembly = (GameObject.Instantiate( Resources.Load( "Tests/BranchedAssemblyMolecule4" ) as GameObject ) as GameObject).GetComponent<AssemblyMolecule>();

		FinderConditional finder = GameObject.FindObjectOfType<FinderConditional>();
		MoleculeBinder binder = GameObject.FindObjectOfType<MoleculeBinder>();

		if (finder.Pass())
		{
			binder.Bind();
		}

		Assert.IsTrue( binder.transform.parent == assembly.transform );

		yield return new WaitForSeconds( 0.1f );

		binder.Release();

		Assert.IsTrue( assembly.rootComponent.transform.parent == assembly.transform );

		yield return null;
	}

	[UnityTest]
	public IEnumerator DoubleBindTest1 ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		MolecularEnvironment.Instance.size = 10f * Vector3.one;

		GameObject.Instantiate( Resources.Load( "Tests/StaticMolecule" ) as GameObject );
		(GameObject.Instantiate( Resources.Load( "Tests/StaticMolecule" ) as GameObject ) as GameObject).transform.position = new Vector3( 0.5f, -1f, 0 );
		AssemblyMolecule assembly = (GameObject.Instantiate( Resources.Load( "Tests/DoubleBindAssemblyMolecule1" ) as GameObject ) as GameObject).GetComponent<AssemblyMolecule>();
		GameObject molecule1 = assembly.transform.GetChild( 0 ).GetChild( 0 ).gameObject;
		GameObject molecule2 = assembly.transform.GetChild( 0 ).GetChild( 1 ).gameObject;

		yield return new WaitForSeconds( 1.1f );

		//molecule1 is bound
		Assert.IsTrue( molecule1.transform.parent == assembly.transform );
		Assert.IsTrue( assembly.rootComponent.transform.parent == molecule1.transform );
		Assert.IsTrue( molecule2.transform.parent == assembly.rootComponent.transform );

		yield return new WaitForSeconds( 1f );

		//both molecules are bound
		Assert.IsTrue( molecule1.transform.parent == assembly.transform );
		Assert.IsTrue( assembly.rootComponent.transform.parent == molecule1.transform );
		Assert.IsTrue( molecule2.transform.parent == assembly.transform );

		yield return new WaitForSeconds( 1f );

		//molecule1 is released leaving only molecule2 bound
		Assert.IsTrue( molecule1.transform.parent == assembly.rootComponent.transform );
		Assert.IsTrue( assembly.rootComponent.transform.parent == molecule2.transform );
		Assert.IsTrue( molecule2.transform.parent == assembly.transform );

		yield return new WaitForSeconds( 1f );

		//molecule2 is released so now it's free
		Assert.IsTrue( molecule1.transform.parent == assembly.rootComponent.transform );
		Assert.IsTrue( assembly.rootComponent.transform.parent == assembly.transform );
		Assert.IsTrue( molecule2.transform.parent == assembly.rootComponent.transform );

		yield return null;
	}

	[UnityTest]
	public IEnumerator DoubleBindTest2 ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		MolecularEnvironment.Instance.size = 20f * Vector3.one;

		GameObject.Instantiate( Resources.Load( "Tests/StaticMolecule" ) as GameObject );
		GameObject.Instantiate( Resources.Load( "Tests/StaticMolecule" ) as GameObject );
		AssemblyMolecule assembly = (GameObject.Instantiate( Resources.Load( "Tests/DoubleBindAssemblyMolecule2" ) as GameObject ) as GameObject).GetComponent<AssemblyMolecule>();
		GameObject molecule8 = assembly.transform.GetChild( 0 ).GetChild( 2 ).GetChild( 0 ).GetChild( 0 ).gameObject;
		GameObject molecule2 = assembly.transform.GetChild( 0 ).GetChild( 0 ).GetChild( 0 ).gameObject;

		yield return new WaitForSeconds( 5.1f );

		//molecule8 is bound
//		Assert.IsTrue( molecule8.transform.parent == assembly.transform );
//		Assert.IsTrue( assembly.rootComponent.transform.parent.parent.parent == molecule8.transform );
//		Assert.IsTrue( molecule2.transform.parent.parent == assembly.rootComponent.transform );

		yield return new WaitForSeconds( 5f );

		//both molecules are bound
//		Assert.IsTrue( molecule8.transform.parent == assembly.transform );
//		Assert.IsTrue( assembly.rootComponent.transform.parent.parent == molecule2.transform );
//		Assert.IsTrue( molecule2.transform.parent == assembly.transform );

		yield return new WaitForSeconds( 5f );

		//molecule8 is released leaving only molecule2 bound
//		Assert.IsTrue( molecule8.transform.parent.parent.parent == assembly.rootComponent.transform );
//		Assert.IsTrue( assembly.rootComponent.transform.parent.parent == molecule2.transform );
//		Assert.IsTrue( molecule2.transform.parent == assembly.transform );

		yield return new WaitForSeconds( 5f );

		//molecule2 is released so now it's free
//		Assert.IsTrue( molecule8.transform.parent.parent.parent == assembly.rootComponent.transform );
//		Assert.IsTrue( assembly.rootComponent.transform.parent == assembly.transform );
//		Assert.IsTrue( molecule2.transform.parent.parent == assembly.rootComponent.transform );

		yield return new WaitForSeconds( 5f );

		yield return null;
	}
}
