using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class AssemblyMolecule : Molecule 
	{
		public Molecule rootComponent;
		public List<ComponentMolecule> componentMolecules = new List<ComponentMolecule>();
		public int boundComponents = 0;

		public override void ParentToBoundMolecule (Molecule _bindingMolecule)
		{
			if (boundComponents == 0)
			{
				base.ParentToBoundMolecule( _bindingMolecule );
			}
		}

		public override void UnParentFromBoundMolecule (Molecule _releasingMolecule)
		{
			if (boundComponents == 1)
			{
				base.UnParentFromBoundMolecule( _releasingMolecule );
			}
			else if (boundComponents > 1)
			{
				List<Molecule> parentedComponents = GetParentedComponents();
				parentedComponents.Remove( _releasingMolecule );
				base.ParentToBoundMolecule( parentedComponents[parentedComponents.GetRandomIndex()] );
			}
		}

		public override void SetToBindingOrientation (MoleculeBinder binder)
		{
			int n = transform.childCount;
			Transform[] children = new Transform[n];
			Vector3[] positions = new Vector3[n];
			Quaternion[] rotations = new Quaternion[n];

			int i = 0;
			foreach (Transform child in transform)
			{
				children[i] = child;
				positions[i] = child.position;
				rotations[i] = child.rotation;
				i++;
			}

			transform.position = binder.boundBinder.molecule.transform.TransformPoint( binder.bindingPosition );
			transform.rotation = binder.boundBinder.molecule.transform.rotation * Quaternion.Euler( binder.bindingRotation );

			for (int j = 0; j < n; j++)
			{
				children[j].position = positions[j];
				children[j].rotation = rotations[j];
			}
		}

		public void UpdateParentScheme ()
		{
			List<Molecule> parentedComponents = GetParentedComponents();
			boundComponents = parentedComponents.Count;
			Debug.Log( "set parenting scheme " + boundComponents );
			if (boundComponents == 0)
			{
				if (rootComponent != null)
				{
					rootComponent.transform.SetParent( transform );
					ParentAllAttachedMoleculesTo( rootComponent, null );
				}
			}
			else if (boundComponents == 1)
			{
				parentedComponents[0].transform.SetParent( transform );
				ParentAllAttachedMoleculesTo( parentedComponents[0], null );
			}
			else if (boundComponents > 1)
			{
				Molecule parentedComponentClosestToRoot = GetComponentClosestToRoot( parentedComponents );

				foreach (Leash leash in rootComponent.leashes)
				{
					Molecule nextToClosest = leash.GetComponentClosestTo( parentedComponentClosestToRoot );
					if (nextToClosest != null)
					{
						nextToClosest.transform.SetParent( parentedComponentClosestToRoot.transform );
						ParentAllAttachedMoleculesTo( nextToClosest, parentedComponentClosestToRoot ); 
					}
				}

				foreach (Molecule parentedComponent in parentedComponents)
				{
					parentedComponent.transform.SetParent( transform );
					ParentAllAttachedMoleculesTo( parentedComponent, GetAttachedMoleculeTowardRoot( parentedComponent ) );
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
			return null;
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
			int n, min = int.MaxValue - 10;
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
						n = leash.GetMinBranchesToComponent( rootComponent );
						if (n < min)
						{
							closestComponent = component;
							min = n;
						}
					}
				}
			}
			return closestComponent;
		}

		void ParentAllAttachedMoleculesTo (Molecule parent, Molecule excludedMolecule)
		{
			foreach (Leash leash in parent.leashes)
			{
				if (leash.attachedMolecule != excludedMolecule)
				{
					leash.attachedMolecule.transform.SetParent( parent.transform );
					ParentAllAttachedMoleculesTo( leash.attachedMolecule, parent );
				}
			}
		}
	}
}