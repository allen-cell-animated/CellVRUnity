using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class MoleculeGenerator : MonoBehaviour 
	{
		[Tooltip("radius in nm")]
		public float generationRadius = 10f;
		[Tooltip("concentration in mM")]
		public float concentration = 5f;
		public Molecule moleculePrefab;

		List<Molecule> molecules = new List<Molecule>();
		float n;
		Transform moleculeParent;

		int _number = -1;
		int number
		{
			get {
				if (_number < 0)
				{
					_number = Mathf.RoundToInt( 4f / 3f * Mathf.PI * concentration * Mathf.Pow( generationRadius, 3f ) * 1E-4f );
				}
				return _number;
			}
		}

		void Start ()
		{
			if (moleculePrefab == null)
			{
				Debug.LogWarning( name + "'s MoleculeGenerator prefab is missing!" );
				enabled = false;
				return;
			}

			moleculeParent = new GameObject( moleculePrefab.name + "Parent" ).transform;
			moleculeParent.position = Vector3.zero;
		}

		void Update ()
		{
			CheckBounds();
			CheckConcentration();
		}

		void CheckBounds ()
		{
			for (int i = 0; i < molecules.Count; i++)
			{
				if (molecules[i] != null && !molecules[i].shouldHide && !molecules[i].hidden)
				{
					if (Vector3.Distance( molecules[i].transform.position, transform.position ) > generationRadius)
					{
						molecules[i].shouldHide = true;
					}
				}
			}
		}

		void CheckConcentration ()
		{
			int n = molecules.FindAll( m => !m.hidden && !m.shouldHide ).Count;
			while (n < number)
			{
				AddMolecule();
				n++;
			}
		}

		void AddMolecule ()
		{
			Vector3 position = transform.position + generationRadius * Random.insideUnitSphere;

			List<Molecule> hiddenMolecules = molecules.FindAll( m => m.hidden );
			if (hiddenMolecules.Count > 0)
			{
				hiddenMolecules[0].transform.position = position;
				hiddenMolecules[0].transform.rotation = Random.rotation;
				hiddenMolecules[0].Regenerate();
			}
			else
			{
				Molecule molecule = Instantiate( moleculePrefab, position, Random.rotation ) as Molecule;
				molecule.parent = moleculeParent;
				molecule.transform.SetParent( moleculeParent );
				molecule.name = moleculePrefab.name + n;
				n++;
				molecules.Add( molecule );
			}
		}

		void OnDrawGizmos ()
		{
			Gizmos.DrawWireSphere( transform.position, generationRadius );
		}
	}
}
