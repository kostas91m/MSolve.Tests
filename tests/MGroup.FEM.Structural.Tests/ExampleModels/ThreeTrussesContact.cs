using MGroup.MSolve.Discretization.Entities;
using MGroup.FEM.Structural.Line;
using MGroup.Constitutive.Structural;
using MGroup.Constitutive.Structural.BoundaryConditions;
using MGroup.Constitutive.Structural.Line;
//using MGroup.Contact.Structural;

namespace MGroup.FEM.Structural.Tests.ExampleModels
{
	public class ThreeTrussesContact
	{
		public static readonly double expected_solution_node4_TranslationX = 0.04967;

		public static Model CreateModel()
		{
			var model = new Model();

			model.SubdomainsDictionary.Add(key: 0, new Subdomain(id: 0));

			var nodes = new[]
			{
				new Node(id: 1, x: 0d, y: 0d),
				new Node(id: 2, x: 5d, y: 0d),
				new Node(id: 3, x: 10d, y: 0d),
				new Node(id: 4, x: 10.1, y: 0d),
				new Node(id: 5, x: 15.1, y: 0d)

			};

			foreach (var node in nodes)
			{
				model.NodesDictionary.Add(node.ID, node);
			}
			for (var i = 0; i < 2; i++)
			{
				var element = new Rod2D(
					new[]
					{
						model.NodesDictionary[i + 1],
						model.NodesDictionary[i + 2]
					},
					youngModulus: 1000d
				)
				{
					ID = i + 1,
					SectionArea = 1d
				};

				model.ElementsDictionary.Add(element.ID, element);
				model.SubdomainsDictionary[0].Elements.Add(element);
			}
			var newElement = new Rod2D(
					new[]
					{
						model.NodesDictionary[4],
						model.NodesDictionary[5]
					},
					youngModulus: 1000d
					)
			{
				ID = 3,
				SectionArea = 1d
			};
			model.ElementsDictionary.Add(newElement.ID, newElement);
			model.SubdomainsDictionary[0].Elements.Add(newElement);
			var contactElement = new ContactNodeToNode2D(
				new[]
				{
					model.NodesDictionary[3],
					model.NodesDictionary[4]
				},
				youngModulus: 1000d,
				penaltyFactorMultiplier: 100d,
				contactArea: 1d
				);
			contactElement.ID = 4;
			model.ElementsDictionary.Add(contactElement.ID, contactElement);
			model.SubdomainsDictionary[0].Elements.Add(contactElement);
			model.BoundaryConditions.Add(new StructuralBoundaryConditionSet(
				new[]
				{
					new NodalDisplacement(model.NodesDictionary[1], StructuralDof.TranslationX, amount: 0d),
					new NodalDisplacement(model.NodesDictionary[1], StructuralDof.TranslationY, amount: 0d),
					new NodalDisplacement(model.NodesDictionary[2], StructuralDof.TranslationY, amount: 0d),
					new NodalDisplacement(model.NodesDictionary[3], StructuralDof.TranslationY, amount: 0d),
					new NodalDisplacement(model.NodesDictionary[4], StructuralDof.TranslationY, amount: 0d),
					new NodalDisplacement(model.NodesDictionary[5], StructuralDof.TranslationX, amount: 0d),
					new NodalDisplacement(model.NodesDictionary[5], StructuralDof.TranslationY, amount: 0d)
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
