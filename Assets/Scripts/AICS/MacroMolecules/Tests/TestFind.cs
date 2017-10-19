using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MacroMolecules;

namespace AICS
{
	public class TestFind : TestComponent 
	{
		public FinderConditional finder;
		public MoleculeBinder binder;

		protected override void Test ()
		{
			if (finder != null && binder != null && finder.Pass())
			{
				binder.Bind();
			}
			PassTest( binder != null && binder.boundMoleculeBinder != null );
		}
	}
}