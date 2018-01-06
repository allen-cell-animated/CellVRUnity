using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class Nucleator : MolecularComponent
	{
		public Polymer polymerPrefab;
		public MoleculeBinder binderForOther;

		public void Nucleate ()
		{
			if (binderForOther != null && binderForOther.boundBinder != null)
			{
				bool otherIsNucleated = (binderForOther.boundBinder.molecule.polymer != null);
				bool thisIsNucleated = (molecule.polymer != null);

				if (otherIsNucleated && !thisIsNucleated)
				{
					JoinPolymer();
				}
				else if (!otherIsNucleated && thisIsNucleated)
				{
					AddMonomer();
				}
				else if (!otherIsNucleated && !thisIsNucleated)
				{
					CreatePolymer();
				}
			}
		}

		protected virtual void JoinPolymer ()
		{
			Polymer polymer = binderForOther.boundBinder.molecule.polymer;

			polymer.monomers.Add( molecule );
			molecule.polymer = polymer;
			molecule.transform.SetParent( polymer.transform );
		}

		protected virtual void AddMonomer ()
		{
			Molecule monomer = binderForOther.boundBinder.molecule;
			Polymer polymer = molecule.polymer;

			polymer.monomers.Add( monomer );
			monomer.polymer = polymer;
			monomer.transform.SetParent( polymer.transform );
		}

		protected virtual void CreatePolymer ()
		{
			if (polymerPrefab != null)
			{
				Polymer polymer = Instantiate( polymerPrefab, transform.position, transform.rotation ) as Polymer;
				polymer.rootMonomer = molecule;

				polymer.monomers.Add( molecule );
				molecule.polymer = polymer;
				molecule.transform.SetParent( polymer.transform );

				if (binderForOther != null && binderForOther.boundBinder != null)
				{
					polymer.monomers.Add( binderForOther.boundBinder.molecule );
					binderForOther.boundBinder.molecule.polymer = polymer;
					binderForOther.boundBinder.molecule.transform.SetParent( polymer.transform );
				}
			}
		}

		public void Dissolve ()
		{
			// TODO
		}
	}
}
