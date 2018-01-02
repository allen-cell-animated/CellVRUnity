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

	public PolarMonomer (string prefabName, Vector3 spawnPosition)
	{
		molecule = (GameObject.Instantiate( Resources.Load( "Tests/" + prefabName ) as GameObject ) as GameObject).GetComponent<Molecule>();
		molecule.transform.position = MolecularEnvironment.Instance.GetRandomPointInBounds();

		foreach (MoleculeBinder binder in molecule.binders)
		{
			if (binder.bindingCriteria.siteId == 0)
			{
				pointedEnd = binder;
			}
			else if (binder.bindingCriteria.siteId == 1)
			{
				barbedEnd = binder;
			}
		}
	}

	public bool BindDirectionsAreCorrect ()
	{
		if (pointedEnd.boundBinder != null)
		{
			if (pointedEnd.boundBinder.bindingCriteria.siteId != 1)
			{
				return false;
			}
		}
		if (barbedEnd.boundBinder != null)
		{
			if (barbedEnd.boundBinder.bindingCriteria.siteId != 0)
			{
				return false;
			}
		}
		return true;
	}
}

public class PolymerizationTests
{
	[UnityTest]
	public IEnumerator DimerizeTest ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		MolecularEnvironment.Instance.size = 10f * Vector3.one;
		PolarMonomer molecule1 = new PolarMonomer( "DimerizeTest", MolecularEnvironment.Instance.GetRandomPointInBounds() );
		PolarMonomer molecule2 = new PolarMonomer( "DimerizeTest", MolecularEnvironment.Instance.GetRandomPointInBounds() );

		while (!molecule1.molecule.bound)
		{
			yield return new WaitForEndOfFrame();
		}

		Assert.IsTrue( molecule1.molecule.bound );
		Assert.IsTrue( molecule1.BindDirectionsAreCorrect() );
		Assert.IsTrue( molecule2.BindDirectionsAreCorrect() );
		Assert.IsTrue( molecule1.molecule.transform.parent == molecule2.molecule.transform || molecule2.molecule.transform.parent == molecule1.molecule.transform );

		yield return null;
	}
}
