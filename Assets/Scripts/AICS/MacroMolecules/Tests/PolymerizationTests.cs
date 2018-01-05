using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using AICS.MacroMolecules;

public class PolarMonomer
{
	public Molecule molecule;
	public MoleculeBinder pointedEnd;
	public MoleculeBinder barbedEnd;

	public PolarMonomer (string prefabName, Vector3 spawnPosition, string objectName)
	{
		molecule = (GameObject.Instantiate( Resources.Load( "Tests/Polymerization/" + prefabName ) as GameObject ) as GameObject).GetComponent<Molecule>();
		molecule.transform.position = MolecularEnvironment.Instance.GetRandomPointInBounds();
		molecule.name = objectName;

		foreach (MoleculeBinder binder in molecule.binders)
		{
			if (binder.thisCriteria.siteId == 0)
			{
				pointedEnd = binder;
			}
			else if (binder.thisCriteria.siteId == 1)
			{
				barbedEnd = binder;
			}
		}
	}

	public bool bindDirectionsAreCorrect
	{
		get
		{
			if (pointedEnd.boundBinder != null)
			{
				if (pointedEnd.boundBinder.thisCriteria.siteId != 1)
				{
					return false;
				}
			}
			if (barbedEnd.boundBinder != null)
			{
				if (barbedEnd.boundBinder.thisCriteria.siteId != 0)
				{
					return false;
				}
			}
			return true;
		}
	}

	public bool stateIsCorrect
	{
		get
		{
			if (pointedEnd.boundBinder != null && barbedEnd.boundBinder != null)
			{
				return molecule.GetMolecularComponents<StateMachine>()[0].currentState.id == 3;
			}
			else if (pointedEnd.boundBinder == null && barbedEnd.boundBinder != null)
			{
				return molecule.GetMolecularComponents<StateMachine>()[0].currentState.id == 2;
			}
			else if (pointedEnd.boundBinder != null && barbedEnd.boundBinder == null)
			{
				return molecule.GetMolecularComponents<StateMachine>()[0].currentState.id == 1;
			}
			else 
			{
				return molecule.GetMolecularComponents<StateMachine>()[0].currentState.id == 0;
			}
		}
	}
}

public class PolymerizationTests
{
	PolarMonomer[] CreateMonomers (string prefabName, int n)
	{
		PolarMonomer[] monomers = new PolarMonomer[n];
		for (int i = 0; i < n; i++)
		{
			monomers[i] = new PolarMonomer( prefabName, MolecularEnvironment.Instance.GetRandomPointInBounds(), "monomer_" + i );
		}
		return monomers;
	}

	bool AllAreBound (PolarMonomer[] monomers)
	{
		foreach (PolarMonomer monomer in monomers)
		{
			if (monomer.pointedEnd.boundBinder == null && monomer.barbedEnd.boundBinder == null)
			{
				return false;
			}
		}
		return true;
	}

	[UnityTest]
	public IEnumerator DimerizeSeparate ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		MolecularEnvironment.Instance.size = 10f * Vector3.one;
		PolarMonomer[] monomers = new PolarMonomer[2];
		monomers[0] = new PolarMonomer( "DimerizeBarbed", MolecularEnvironment.Instance.GetRandomPointInBounds(), "barbed" );
		monomers[1] = new PolarMonomer( "DimerizePointed", MolecularEnvironment.Instance.GetRandomPointInBounds(), "pointed" );

		while (!AllAreBound( monomers ))
		{
			yield return new WaitForEndOfFrame();
		}

		Polymer polymer = GameObject.FindObjectOfType<Polymer>();
		foreach (PolarMonomer monomer in monomers)
		{
			Assert.IsTrue( monomer.bindDirectionsAreCorrect );
			Assert.IsTrue( monomer.molecule.transform.parent == polymer.transform );
		}

		yield return null;
	}

	[UnityTest]
	public IEnumerator DimerizeSingle ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		MolecularEnvironment.Instance.size = 10f * Vector3.one;
		PolarMonomer[] monomers = CreateMonomers( "Dimerize", 2 );

		while (!AllAreBound( monomers ))
		{
			yield return new WaitForEndOfFrame();
		}

		Polymer polymer = GameObject.FindObjectOfType<Polymer>();
		foreach (PolarMonomer monomer in monomers)
		{
			Assert.IsTrue( monomer.bindDirectionsAreCorrect );
			Assert.IsTrue( monomer.molecule.transform.parent == polymer.transform );
			Assert.IsTrue( monomer.stateIsCorrect );
		}
		Assert.IsTrue( polymer.monomers.Count == 2 );

		yield return null;
	}

	[UnityTest]
	public IEnumerator Trimerize ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		MolecularEnvironment.Instance.size = 10f * Vector3.one;
		PolarMonomer[] monomers = CreateMonomers( "Trimerize", 3 );

		while (!AllAreBound( monomers ))
		{
			yield return new WaitForEndOfFrame();
		}

		Polymer polymer = GameObject.FindObjectOfType<Polymer>();
		foreach (PolarMonomer monomer in monomers)
		{
			Assert.IsTrue( monomer.bindDirectionsAreCorrect );
			Assert.IsTrue( monomer.molecule.transform.parent == polymer.transform );
			Assert.IsTrue( monomer.stateIsCorrect );
		}
		Assert.IsTrue( polymer.monomers.Count == 3 );

		yield return null;
	}

	[UnityTest]
	public IEnumerator Polymerize ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		MolecularEnvironment.Instance.size = 30f * Vector3.one;
		PolarMonomer[] monomers = CreateMonomers( "Trimerize", 60 );

		while (!AllAreBound( monomers ))
		{
			yield return new WaitForEndOfFrame();
		}

		foreach (PolarMonomer monomer in monomers)
		{
			Assert.IsTrue( monomer.bindDirectionsAreCorrect );
			Assert.IsNotNull( monomer.molecule.transform.parent.GetComponent<Polymer>() );
			Assert.IsTrue( monomer.stateIsCorrect );
		}

		yield return null;
	}
}
