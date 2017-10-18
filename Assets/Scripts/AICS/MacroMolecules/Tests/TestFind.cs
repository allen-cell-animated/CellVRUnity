using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MacroMolecules;

public class TestFind : MonoBehaviour 
{
	public FinderConditional finder;
	public MoleculeBinder binder;

	bool initialized = false;

	void Update () 
	{
		if (!initialized)
		{
			Init();
		}
		if (binder != null && binder.boundMoleculeBinder != null)
		{
			IntegrationTest.Pass();
		}
	}

	void Init ()
	{
		if (finder == null)
		{
			IntegrationTest.Fail( "No finder!" );
		}
		else if (binder == null)
		{
			IntegrationTest.Fail( "No binder!" );
		}
		else if (!finder.Pass())
		{
			IntegrationTest.Fail( "finder found nothing!" );
		}
		else
		{
			binder.Bind();
		}
		initialized = true;
	}
}
