using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class ComponentSpawner : AssemblyMolecularComponent, ISetup
	{
		public List<ComponentMolecule> componentPrefabs = new List<ComponentMolecule>();

		Transform _componentParent;
		protected Transform componentParent
		{
			get
			{
				if (_componentParent == null)
				{
					_componentParent = new GameObject( "Components" ).transform;
					_componentParent.SetParent( transform );
					_componentParent.localPosition = Vector3.zero;
					_componentParent.localRotation = Quaternion.identity;
				}
				return _componentParent;
			}
		}

		public void DoSetup ()
		{
			SpawnAll();
		}

		protected virtual void SpawnAll ()
		{
			foreach (ComponentMolecule prefab in componentPrefabs)
			{
				SpawnComponent( prefab );
			}
		}

		protected ComponentMolecule SpawnComponent (ComponentMolecule prefab, int index = 0)
		{
			if (prefab != null)
			{
				ComponentMolecule _molecule = Instantiate( prefab, componentParent );
				assemblyMolecule.componentMolecules.Add( _molecule );
				_molecule.assembly = assemblyMolecule;
				_molecule.name = prefab.name + "_" + index;
				return _molecule;
			}
			return null;
		}

		protected void PlaceComponent (ComponentMolecule component, Vector3 position, Vector3 lookDirection, Vector3 normal)
		{
			component.transform.localPosition = position;
			component.transform.LookAt( component.transform.position + lookDirection, normal );
		}
	}
}