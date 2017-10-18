using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MacroMolecules;

public class TestFindFront : MonoBehaviour 
{
	public FrontFinderConditional finder;
	public Molecule moleculeToFindMost;
	public int attempts;

	int n = 0;
	int foundGoal;
	int foundOther;

	void Update ()
	{
		while (n < attempts)
		{
			AttemptToFind();
			n++;
		}

		if (foundGoal > foundOther)
		{
			IntegrationTest.Pass();
		}
		else
		{
			IntegrationTest.Fail();
		}
	}

	void AttemptToFind ()
	{
		if (finder != null)
		{
			IBind found = finder.Find();
			if (found != null && found.molecule == moleculeToFindMost)
			{
				foundGoal++;
			}
			else 
			{
				foundOther++;
			}
		}
	}
}
