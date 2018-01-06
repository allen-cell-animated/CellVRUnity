using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class MoleculeDetector : MonoBehaviour 
	{
		public List<Molecule> molecules = new List<Molecule>();
		public MoleculeType[] moleculesToFind;

		List<Molecule> nearbyMolecules = new List<Molecule>();

		SphereCollider _collider;
		SphereCollider theCollider
		{
			get
			{
				if (_collider == null)
				{
					_collider = GetComponent<SphereCollider>();
				}
				return _collider;
			}
		}

		public float radius
		{
			get
			{
				return theCollider.radius;
			}
		}

		public MoleculeDetector Setup (MoleculeType[] _moleculesToFind, float _radius)
		{
			moleculesToFind = _moleculesToFind;
			transform.localPosition = Vector3.zero;
			SetRadius( _radius );

			string n = "";
			foreach (MoleculeType type in moleculesToFind)
			{
				n += type.ToString() + "/";
			}
			name = n + "Detector";

			return this;
		}

		void OnTriggerEnter (Collider other)
		{
			Molecule molecule = other.GetComponentInParent<Molecule>();
			if (molecule != null && !molecules.Contains( molecule ) && MoleculeIsATypeToFind( molecule ))
			{
				molecules.Add( molecule );
			}
		}

		bool MoleculeIsATypeToFind (Molecule _molecule)
		{
			foreach (MoleculeType typeToFind in moleculesToFind)
			{
				if (typeToFind == MoleculeType.All || typeToFind == _molecule.type)
				{
					return true;
				}
			}
			return false;
		}

		void OnTriggerExit (Collider other)
		{
			Molecule molecule = other.GetComponentInParent<Molecule>();
			if (molecule != null && molecules.Contains( molecule ))
			{
				molecules.Remove( molecule );
			}
		}

		public List<Molecule> GetNearbyMolecules (Vector3 position, float radius)
		{
			nearbyMolecules.Clear();
			foreach (Molecule m in molecules)
			{
				if (Vector3.Distance( m.transform.position, position ) <= m.radius + radius)
				{
					nearbyMolecules.Add( m );
				}
			}
			return nearbyMolecules;
		}

		public bool WillCollide (Vector3 position, float radius)
		{
			return GetNearbyMolecules( position, radius ).Count > 0;
		}

		public void SetRadius (float _radius)
		{
			theCollider.radius = _radius;
		}
	}
}