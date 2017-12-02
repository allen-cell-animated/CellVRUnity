using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.PhysicsKinesin
{
	public class NucleotideGenerator : MonoBehaviour
	{
		[Tooltip("radius in nm")]
		public float generationRadius = 10f;
		[Tooltip("concentration in mM")]
		public float concentration = 5f;
		public Nucleotide moleculePrefab;
		public Transform moleculeParent;

		List<Nucleotide> molecules = new List<Nucleotide>();
		float n;
		float moleculeMass = -1f;
		bool destroyed = false;

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

			if (moleculePrefab.body != null)
			{
				moleculeMass = moleculePrefab.body.mass;
			}
		}

		void Update ()
		{
			if (!destroyed)
			{
				CheckBounds();
				CheckConcentration();
			}
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
			int n = molecules.FindAll( m => !m.hidden && !m.shouldHide && m.isATP ).Count;
			while (n < number)
			{
				AddMolecule();
				n++;
			}
		}

		void AddMolecule ()
		{
			Vector3 position = transform.position + generationRadius * Random.insideUnitSphere;

			List<Nucleotide> hiddenMolecules = molecules.FindAll( m => m.hidden );
			if (hiddenMolecules.Count > 0)
			{
				hiddenMolecules[0].transform.position = position;
				hiddenMolecules[0].transform.rotation = Random.rotation;
				hiddenMolecules[0].Regenerate();
			}
			else
			{
				Nucleotide molecule = Instantiate( moleculePrefab, position, Random.rotation ) as Nucleotide;
				molecule.parent = moleculeParent;
				molecule.transform.SetParent( moleculeParent );
				molecule.name = moleculePrefab.name + n;
				molecule.mass = molecule.body.mass = moleculeMass;
				n++;
				molecules.Add( molecule );
			}
		}

		void OnDrawGizmos ()
		{
			Gizmos.DrawWireSphere( transform.position, generationRadius );
		}

		public void SetMoleculeMass (float mass)
		{
			if (moleculeMass >= 0 && (mass < moleculeMass - 0.1f || mass > moleculeMass + 0.1f))
			{
				moleculeMass = mass;
				foreach (Nucleotide molecule in molecules)
				{
					if (molecule.body != null)
					{
						molecule.mass = molecule.body.mass = moleculeMass;
					}
				}
			}
		}

		public Nucleotide GetClosestATPToPoint (Vector3 point)
		{
			float d, min = Mathf.Infinity;
			Nucleotide closest = null;
			foreach (Nucleotide molecule in molecules)
			{
				if ((molecule as Nucleotide).isATP)
				{
					d = Vector3.Distance( molecule.transform.position, point );
					if (d < min)
					{
						min = d;
						closest = molecule;
					}
				}
			}
			return closest;
		}

		public void DoReset ()
		{
			foreach (Nucleotide molecule in molecules)
			{
				molecule.Restart();
			}
		}

		public void DestroyAll ()
		{
			destroyed = true;
			foreach (Nucleotide molecule in molecules)
			{
				Destroy( molecule.gameObject );
			}
		}
	}
}
