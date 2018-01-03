using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using AICS.MacroMolecules;
using AICS.MacroMolecules.Extensions;

namespace AICS.MacroMolecules.Extensions
{
	public static class BinderExtensions
	{
		public static MoleculeBinder GetOtherBinder (this MoleculeBinder binder)
		{
			foreach (MoleculeBinder b in binder.molecule.binders)
			{
				if (b != binder)
				{
					return b;
				}
			}
			return null;
		}

		public static bool IsParent (this MoleculeBinder binder)
		{
			return binder.boundBinder != null && binder.molecule.transform.parent == binder.boundBinder.molecule.transform;
		}

		public static bool IsPointedEnd (this MoleculeBinder binder)
		{
			return binder.thisCriteria.siteId == 0;
		}
	}
}

public class PolarMonomer
{
	public Molecule molecule;
	public MoleculeBinder pointedEnd;
	public MoleculeBinder barbedEnd;

	public PolarMonomer (string prefabName, Vector3 spawnPosition)
	{
		molecule = (GameObject.Instantiate( Resources.Load( "Tests/Polymerization/" + prefabName ) as GameObject ) as GameObject).GetComponent<Molecule>();
		molecule.transform.position = MolecularEnvironment.Instance.GetRandomPointInBounds();

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

	public MoleculeBinder parentedBinder
	{
		get
		{
			if (molecule.transform.parent != null)
			{
				foreach (MoleculeBinder binder in molecule.binders)
				{
					if (molecule.transform.parent == binder.boundBinder.molecule.transform)
					{
						return binder;
					}
				}
			}
			return null;
		}
	}
}

public class PolymerizationTests
{
	PolarMonomer[] CreateMonomers (int n)
	{
		PolarMonomer[] monomers = new PolarMonomer[n];
		for (int i = 0; i < n; i++)
		{
			monomers[i] = new PolarMonomer( "PolarMonomerBind", MolecularEnvironment.Instance.GetRandomPointInBounds() );
		}
		return monomers;
	}

	PolarMonomer GetAMiddleMonomer (PolarMonomer[] monomers)
	{
		foreach (PolarMonomer monomer in monomers)
		{
			if (monomer.pointedEnd.boundBinder != null && monomer.barbedEnd.boundBinder != null)
			{
				return monomer;
			}
		}
		return null;
	}

	bool CheckParentRecursively (MoleculeBinder binder)
	{
		if (binder.IsParent())
		{
			//TODO
		}
	}

	[UnityTest]
	public IEnumerator Dimerize ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		MolecularEnvironment.Instance.size = 10f * Vector3.one;
		PolarMonomer[] monomers = CreateMonomers( 2 );

		while (!monomers[0].molecule.bound)
		{
			yield return new WaitForEndOfFrame();
		}

		foreach (PolarMonomer monomer in monomers)
		{
			Assert.IsTrue( monomer.bindDirectionsAreCorrect );
		}
		Assert.IsTrue( monomers[0].molecule.transform.parent == monomers[1].molecule.transform || monomers[1].molecule.transform.parent == monomers[0].molecule.transform );

		yield return null;
	}

	[UnityTest]
	public IEnumerator Trimerize ()
	{
		new GameObject( "MolecularEnvironment", typeof(MolecularEnvironment) );
		MolecularEnvironment.Instance.size = 10f * Vector3.one;
		PolarMonomer[] monomers = CreateMonomers( 3 );

		PolarMonomer middle = null;
		while (middle == null)
		{
			middle = GetAMiddleMonomer( monomers );

			yield return new WaitForEndOfFrame();
		}

		foreach (PolarMonomer monomer in monomers)
		{
			Assert.IsTrue( monomer.bindDirectionsAreCorrect );
		}


		yield return null;
	}
}
