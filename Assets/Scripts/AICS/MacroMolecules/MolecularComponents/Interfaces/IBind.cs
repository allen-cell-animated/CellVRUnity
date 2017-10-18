using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public interface IBind 
	{
		MoleculeType typeToBind { get; }

		Molecule molecule { get; }

		IBind boundMoleculeBinder { get; set; }

		bool parentToBoundMolecule { get; }

		void Bind();

		void MoveMoleculeToBindingPosition();
	}
}