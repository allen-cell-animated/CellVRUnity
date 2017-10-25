using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class AssemblyMolecule : Molecule 
	{
		public Molecule rootComponent;
		public List<ComponentMolecule> componentMolecules = new List<ComponentMolecule>();

		public void UpdateParentScheme ()
		{
			List<Molecule> parentedComponents = GetParentedComponents();
			if (parentedComponents.Count == 0)
			{
				if (rootComponent != null)
				{
					rootComponent.transform.SetParent( transform );
					SetParent( rootComponent, null );
				}
			}
			else if (parentedComponents.Count == 1)
			{
				parentedComponents[0].transform.SetParent( transform );
				SetParent( parentedComponents[0], null );
			}
			else
			{
				//todo
				Molecule parentedComponentClosestToRoot = GetComponentClosestToRoot( parentedComponents );

			}
		}

		List<Molecule> GetParentedComponents ()
		{
			List<Molecule> parentedComponents = new List<Molecule>();
			foreach (Molecule component in componentMolecules)
			{
				if (component.GetParentedBinder() != null)
				{
					parentedComponents.Add( component );
				}
			}
			return parentedComponents;
		}

		Molecule GetComponentClosestToRoot (List<Molecule> components)
		{
			float n, min = Mathf.Infinity;
			Molecule closestComponent;
			foreach (Molecule component in components)
			{
				n = GetMinBranchesToRoot( component, null );
				if (n < min)
				{
					closestComponent = component;
					min = n;
				}
			}
			return closestComponent;
		}

		float GetMinBranchesToRoot (Molecule parent, Molecule grandparent)
		{
			if (parent == rootComponent)
			{
				return 0;
			}

			float n, min = Mathf.Infinity;
			foreach (Leash leash in parent.leashes)
			{
				if (leash.attachedMolecule != grandparent)
				{
					n = GetMinBranchesToRoot( leash.attachedMolecule, parent );
					if (n < min)
					{
						min = n;
					}
				}
			}
			return min + 1f;
		}

		bool BranchContainsComponent (Leash leash, Molecule componentToFind)
		{
			foreach (Leash l in leash.attachedMolecule.leashes)
			{
				if (l.attachedMolecule != leash.molecule)
				{
					if (l.attachedMolecule == componentToFind)
					{
						return true;
					}

				}
			}
		}

		void SetParent (Molecule parent, Molecule grandparent)
		{
			List<Leash> leashes = parent.leashes;
			foreach (Leash leash in leashes)
			{
				if (leash.attachedMolecule != grandparent)
				{
					leash.attachedMolecule.transform.SetParent( parent.transform );
					SetParent( leash.attachedMolecule, parent );
				}
			}
		}
	}
}