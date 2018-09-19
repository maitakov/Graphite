/******************************************************************************
  Copyright 2009-2014 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.FhisPrimitives
{

	public static class NShapeLibraryInitializer
	{

		public static void Initialize(IRegistrar registrar)
		{
			if(registrar == null) throw new ArgumentNullException("registrar");
			// Register library
			registrar.RegisterLibrary(namespaceName, preferredRepositoryVersion);

			// Register linear shapes
			registrar.RegisterShapeType(new ShapeType("Полилиния", namespaceName, resourceString,   // "Polyline"
				Polyline.CreateInstance, Polyline.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("ПрямоугольнаяЛиния", namespaceName, resourceString,  // "RectangularLine"
				RectangularLine.CreateInstance, RectangularLine.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Дуга", namespaceName, resourceString,    // "CircularArc"
				"With only two points, it behaves like a straight line, with all three points, it behaves like a circular arc.",
				CircularArc.CreateInstance, CircularArc.GetPropertyDefinitions));

			// Register text shapes
			registrar.RegisterShapeType(new ShapeType("Текст", namespaceName, resourceString,   // "Text"
				"Supports automatic sizing to its text.",
				Text.CreateInstance, Text.GetPropertyDefinitions,
				Properties.Resources.ShaperReferenceQuadrangle));
			registrar.RegisterShapeType(new ShapeType("Метка", namespaceName, resourceString,   // "Label"
				"Supports autosizing to its text and connecting to other shapes. If the label's 'pin' is connected to a shape, the label will move with its partner shape.",
				Label.CreateInstance, Label.GetPropertyDefinitions,
				Properties.Resources.ShaperReferenceQuadrangle));

			registrar.RegisterShapeType(new ShapeType("Многогранник", namespaceName, resourceString,    // "RegularPolygone"
				RegularPolygone.CreateInstance, RegularPolygone.GetPropertyDefinitions));

			// Register triangle shapes
			registrar.RegisterShapeType(new ShapeType("Треугольник", namespaceName, resourceString, // "FreeTriangle"
				FreeTriangle.CreateInstance, FreeTriangle.GetPropertyDefinitions,
				Properties.Resources.ShaperReferenceTriangle));
			registrar.RegisterShapeType(new ShapeType("РавнобедренныйТреугольник", namespaceName, resourceString,   // "IsoscelesTriangle"
				IsoscelesTriangle.CreateInstance, IsoscelesTriangle.GetPropertyDefinitions,
				Properties.Resources.ShaperReferenceTriangle));

			// Register round shapes
			registrar.RegisterShapeType(new ShapeType("Круг", namespaceName, resourceString,    // "Circle"
				Circle.CreateInstance, Circle.GetPropertyDefinitions,
				Properties.Resources.ShaperReferenceCircle));
			registrar.RegisterShapeType(new ShapeType("Эллипс", namespaceName, resourceString,  // "Ellipse"
				Ellipse.CreateInstance, Ellipse.GetPropertyDefinitions,
				Properties.Resources.ShaperReferenceCircle));

			// Register quadrangle shapes
			registrar.RegisterShapeType(new ShapeType("Квадрат", namespaceName, resourceString, // "Square"
				Square.CreateInstance, Square.GetPropertyDefinitions,
				Properties.Resources.ShaperReferenceQuadrangle));
			registrar.RegisterShapeType(new ShapeType("Прямоугольник", namespaceName, resourceString,   // "Box"
				Box.CreateInstance, Box.GetPropertyDefinitions,
				Properties.Resources.ShaperReferenceQuadrangle));
			registrar.RegisterShapeType(new ShapeType("СкруглённыйПрямоугольник", namespaceName, resourceString,    // "RoundedBox"
				RoundedBox.CreateInstance, RoundedBox.GetPropertyDefinitions,
				Properties.Resources.ShaperReferenceQuadrangle));
			registrar.RegisterShapeType(new ShapeType("Ромб", namespaceName, resourceString,    // "Diamond"
				Diamond.CreateInstance, Diamond.GetPropertyDefinitions,
				Properties.Resources.ShaperReferenceDiamond));

			// Register other shapes
			registrar.RegisterShapeType(new ShapeType("Стрела", namespaceName, resourceString,  // "ThickArrow"
				delegate (ShapeType shapeType, Template t) { return (Shape)new ThickArrow(shapeType, t); },
				ThickArrow.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Изображение", namespaceName, resourceString, // "Picture"
				Picture.CreateInstance, Picture.GetPropertyDefinitions,
				Properties.Resources.ShaperReferenceQuadrangle));
		}


		#region Fields

		private const string namespaceName = "FhisPrimitives";
		private const string resourceString = "Графические примитивы";

		private const int preferredRepositoryVersion = 5;

		#endregion
	}

}
