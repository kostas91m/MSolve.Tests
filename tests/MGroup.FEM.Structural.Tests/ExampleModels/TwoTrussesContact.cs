using MGroup.MSolve.Discretization.Entities;
using MGroup.FEM.Structural.Line;
using MGroup.Constitutive.Structural;
using MGroup.Constitutive.Structural.BoundaryConditions;
using MGroup.Constitutive.Structural.Line;

namespace MGroup.FEM.Structural.Tests.ExampleModels
{
	public class TwoTrussesContact
	{
		public static readonly double expected_solution_node3_TranslationX = 0.075;

		public static Model CreateModel()
		{
			var model = new Model();

			model.SubdomainsDictionary.Add(key: 0, new Subdomain(id: 0));

			var nodes = new[]
			{
				new Node(id: 1, x: 0d, y: 0d),
				new Node(id: 2, x: 5d, y: 0d),
				new Node(id: 3, x: 5.1, y: 0d),
				new Node(id: 4, x: 10.1, y: 0d)
			};

			foreach (var node in nodes)
			{
				model.NodesDictionary.Add(node.ID, node);
			}
			var element1 = new Rod2D(
				new[]
				{
					model.NodesDictionary[1],
					model.NodesDictionary[2]
				},
			youngModulus: 1000d)
			{
				ID = 1,
				SectionArea = 1d
			};
			model.ElementsDictionary.Add(element1.ID, element1);
			model.SubdomainsDictionary[0].Elements.Add(element1);
			var element2 = new Rod2D(
					new[]
					{
						model.NodesDictionary[3],
						model.NodesDictionary[4]
					},
					youngModulus: 1000d
					)
			{
				ID = 2,
				SectionArea = 1d
			};
			model.ElementsDictionary.Add(element2.ID, element2);
			model.SubdomainsDictionary[0].Elements.Add(element2);
			var contactElement = new ContactNodeToNode2D(
				new[]
				{
					model.NodesDictionary[2],
					model.NodesDictionary[3]
				},
				youngModulus: 1000d,
				penaltyFactorMultiplier: 100d,
				contactArea: 1d
				);
			contactElement.ID = 3;
			model.ElementsDictionary.Add(contactElement.ID, contactElement);
			model.SubdomainsDictionary[0].Elements.Add(contactElement);
			model.BoundaryConditions.Add(new StructuralBoundaryConditionSet(
				new[]
				{
					new NodalDisplacement(model.NodesDictionary[1], StructuralDof.TranslationX, amount: 0d),
					new NodalDisplacement(model.NodesDictionary[1], StructuralDof.TranslationY, amount: 0d),
					new NodalDisplacement(model.NodesDictionary[2], StructuralDof.TranslationY, amount: 0d),
					new NodalDisplacement(model.NodesDictionary[3], StructuralDof.TranslationY, amount: 0d),
					new NodalDisplacement(model.NodesDictionary[4], StructuralDof.TranslationX, amount: 0d),
					new NodalDisplacement(model.NodesDictionary[4], StructuralDof.TranslationY, amount: 0d)
				},
				new[]
				{
					new NodalLoad(model.NodesDictionary[2], StructuralDof.TranslationX, amount: 50d)
				}
			));

			return model;
		}
	}
}
