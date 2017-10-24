using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class AssemblyMolecule : Molecule 
	{
		public ComponentMolecule rootComponent;
		public List<ComponentMolecule> componentMolecules = new List<ComponentMolecule>();

		public void UpdateParentScheme ()
		{
			List<ComponentMolecule> parentedComponents = GetParentedComponents();
			if (parentedComponents.Count == 0)
			{
				if (rootComponent != null)
				{
					rootComponent.transform.SetParent( transform );
					SetParentRecursively( rootComponent, null );
				}
			}
			else if (parentedComponents.Count == 1)
			{
				parentedComponents[0].transform.SetParent( transform );
				SetParentRecursively( parentedComponents[0], null );
			}
			else
			{
				ComponentMolecule componentClosestToRoot = GetComponentClosestToRoot( parentedComponents );
				//todo
			}
		}

		List<ComponentMolecule> GetParentedComponents ()
		{
			List<ComponentMolecule> parentedComponents = new List<ComponentMolecule>();
			foreach (ComponentMolecule component in componentMolecules)
			{
				if (component.GetParentedBinder() != null)
				{
					parentedComponents.Add( component );
				}
			}
			return parentedComponents;
		}

		ComponentMolecule GetComponentClosestToRoot (List<ComponentMolecule> parentedComponents)
		{
			List<int> branchesToRoot = new List<int>();
			foreach (ComponentMolecule component in parentedComponents)
			{
				foreach (Leash leash in component.leashes)
				{
					// is root in this branch? check recursively
				}
			}
			return null;
		}

		void SetParentRecursively (Molecule parent, Molecule grandparent)
		{
			List<Leash> leashes = parent.leashes;
			foreach (Leash leash in leashes)
			{
				if (leash.attachedMolecule != grandparent)
				{
					leash.attachedMolecule.transform.SetParent( parent.transform );
					SetParentRecursively( leash.attachedMolecule, parent );
				}
			}
		}
	}
}