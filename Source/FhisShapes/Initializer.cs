using System;
using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.FhisShapes
{
	public static class NShapeLibraryInitializer
	{

		public static void Initialize(IRegistrar registrar)
		{
			if (registrar == null) throw new ArgumentNullException("registrar");
			// Register library
			registrar.RegisterLibrary(namespaceName, preferredRepositoryVersion);


			//	// Register linear shapes
			//	registrar.RegisterShapeType(new ShapeType("Polyline", namespaceName, namespaceName,
			//		Polyline.CreateInstance, Polyline.GetPropertyDefinitions));
			//	registrar.RegisterShapeType(new ShapeType("RectangularLine", namespaceName, namespaceName,
			//		RectangularLine.CreateInstance, RectangularLine.GetPropertyDefinitions));
			//	registrar.RegisterShapeType(new ShapeType("CircularArc", namespaceName, namespaceName,
			//		"With only two points, it behaves like a straight line, with all three points, it behaves like a circular arc.",
			//		CircularArc.CreateInstance, CircularArc.GetPropertyDefinitions));
			//
			//	// Register text shapes
			//	registrar.RegisterShapeType(new ShapeType("Text", namespaceName, namespaceName,
			//		"Supports automatic sizing to its text.",
			//		Text.CreateInstance, Text.GetPropertyDefinitions,
			//		Properties.Resources.ShaperReferenceQuadrangle));
			//	registrar.RegisterShapeType(new ShapeType("Label", namespaceName, namespaceName,
			//		"Supports autosizing to its text and connecting to other shapes. If the label's 'pin' is connected to a shape, the label will move with its partner shape.",
			//		Label.CreateInstance, Label.GetPropertyDefinitions,
			//		Properties.Resources.ShaperReferenceQuadrangle));
			//
			//	registrar.RegisterShapeType(new ShapeType("RegularPolygone", namespaceName, namespaceName,
			//		RegularPolygone.CreateInstance, RegularPolygone.GetPropertyDefinitions));

			// Register triangle shapes
			//	registrar.RegisterShapeType(new ShapeType("FreeTriangle", namespaceName, namespaceName,
			//		FreeTriangle.CreateInstance, FreeTriangle.GetPropertyDefinitions,
			//		Properties.Resources.ShaperReferenceTriangle));
			//	registrar.RegisterShapeType(new ShapeType("IsoscelesTriangle", namespaceName, namespaceName,
			//		IsoscelesTriangle.CreateInstance, IsoscelesTriangle.GetPropertyDefinitions,
			//		Properties.Resources.ShaperReferenceTriangle));

			// Register round shapes
			//	registrar.RegisterShapeType(new ShapeType("Circle", namespaceName, namespaceName,
			//		Circle.CreateInstance, Circle.GetPropertyDefinitions,
			//		Properties.Resources.ShaperReferenceCircle));
			//	registrar.RegisterShapeType(new ShapeType("Ellipse", namespaceName, namespaceName,
			//		Ellipse.CreateInstance, Ellipse.GetPropertyDefinitions,
			//		Properties.Resources.ShaperReferenceCircle));

			// Register quadrangle shapes
			//	registrar.RegisterShapeType(new ShapeType("Square", namespaceName, namespaceName,
			//		Square.CreateInstance, Square.GetPropertyDefinitions,
			//		Properties.Resources.ShaperReferenceQuadrangle));
			//	registrar.RegisterShapeType(new ShapeType("Box", namespaceName, namespaceName,
			//		Box.CreateInstance, Box.GetPropertyDefinitions,
			//		Properties.Resources.ShaperReferenceQuadrangle));
			//	registrar.RegisterShapeType(new ShapeType("RoundedBox", namespaceName, namespaceName,
			//		RoundedBox.CreateInstance, RoundedBox.GetPropertyDefinitions,
			//		Properties.Resources.ShaperReferenceQuadrangle));
			//	registrar.RegisterShapeType(new ShapeType("Diamond", namespaceName, namespaceName,
			//		Diamond.CreateInstance, Diamond.GetPropertyDefinitions,
			//		Properties.Resources.ShaperReferenceDiamond));

			// Register other shapes
			//	registrar.RegisterShapeType(new ShapeType("ThickArrow", namespaceName, namespaceName,
			//		delegate (ShapeType shapeType, Template t) { return (Shape)new ThickArrow(shapeType, t); },
			//		ThickArrow.GetPropertyDefinitions));
			//	registrar.RegisterShapeType(new ShapeType("Picture", namespaceName, namespaceName,
			//		Picture.CreateInstance, Picture.GetPropertyDefinitions,
			//		Properties.Resources.ShaperReferenceQuadrangle));

			registrar.RegisterShapeType(new ShapeType("Ресурс", namespaceName, resourceString,
				Resource.CreateInstance, Resource.GetPropertyDefinitions,
				Properties.Resources.ShaperReferenceQuadrangle));

			registrar.RegisterShapeType(new ShapeType("Свойство", namespaceName, resourceString,
				Property.CreateInstance, Property.GetPropertyDefinitions,
				Properties.Resources.ShaperReferenceTriangle));

			registrar.RegisterShapeType(new ShapeType("Действие", namespaceName, resourceString,
				delegate (ShapeType shapeType, Template t) { return (Shape)new Action(shapeType, t); },
				Action.GetPropertyDefinitions));
		}


		#region Fields

		private const string namespaceName = "FhisShapes";
		private const string resourceString = "Базовые формы";	//"ФГиИС";

		private const int preferredRepositoryVersion = 5;

		#endregion
	}
}
