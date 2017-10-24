using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MacroMolecules;

namespace AICS
{
	public class TestLeash : TestComponent 
	{
		public AssemblyMolecule assembly;
		public ComponentMolecule moleculeToBind;
		public ComponentMolecule defaultParentsParentWhenBound;

		protected override void TestOnce ()
		{
//			PassTest( TestBind() && TestRelease() );
		}

//		bool TestBind ()
//		{
//			assembly.SetParentSchemeOnComponentBind( moleculeToBind );
//			return moleculeToBind.transform.parent == assembly.transform && assembly.defaultParent.transform.parent == defaultParentsParentWhenBound.transform;
//		}
//
//		bool TestRelease ()
//		{
//			assembly.SetParentSchemeOnComponentRelease( moleculeToBind );
//			return assembly.defaultParent.transform.parent == assembly.transform;
//		}

		protected override void TestUntilPass () {}
	}
}