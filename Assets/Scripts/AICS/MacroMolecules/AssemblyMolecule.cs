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
				Molecule parentedComponentClosestToRoot = GetComponentClosestToRoot( parentedComponents );

				foreach (Leash leash in rootComponent.leashes)
				{
					Molecule nextToClosest = leash.GetComponentClosestTo( parentedComponentClosestToRoot );
					if (nextToClosest != null)
					{
						nextToClosest.transform.SetParent( parentedComponentClosestToRoot.transform );
						SetParent( nextToClosest, parentedComponentClosestToRoot ); 
					}
				}

				foreach (Molecule parentedComponent in parentedComponents)
				{
					parentedComponent.transform.SetParent( transform );
					SetParent( parentedComponent, GetAttachedMoleculeTowardRoot( parentedComponent ) );
				}
			}
		}

		Molecule GetAttachedMoleculeTowardRoot (Molecule parent)
		{
			foreach (Leash leash in parent.leashes)
			{
				if (leash.GetComponentClosestTo( rootComponent ) != null)
				{
					return leash.attachedMolecule;
				}
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
			int n, min = (int)Mathf.Infinity;
			Molecule closestComponent = null;
			foreach (Molecule component in components)
			{
				if (component == rootComponent)
				{
					n = 0;
				}
				else 
				{
					foreach (Leash leash in component.leashes)
					{
						n = leash.GetMinBranchesToComponent( rootComponent ) + 1;
					}
				}

				if (n < min)
				{
					closestComponent = component;
					min = n;
				}
			}
			return closestComponent;
		}

		void SetParent (Molecule parent, Molecule grandparent)
		{
			foreach (Leash leash in parent.leashes)
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