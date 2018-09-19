using System;
using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.FhisServices
{
	public static class NShapeLibraryInitializer
	{

		public static void Initialize(IRegistrar registrar)
		{
			if (registrar == null) throw new ArgumentNullException("registrar");
			if(registrar is Project)
				foreach(ShapeType shapeType in ((Project)registrar).ShapeTypes)
					switch(shapeType.FullName)
					{	case "FhisPrimitives.Круг":
							Watch.CircleType = shapeType;
							Thermometer.CircleType = shapeType;
							Barometer.CircleType = shapeType;
							break;
						case "FhisPrimitives.Прямоугольник":
							Calendar.BoxType = shapeType;
							Thermometer.BoxType = shapeType;
							break;
						case "FhisPrimitives.Полилиния":
							Watch.PolylineType = shapeType;
							Windvane.PolylineType = shapeType;
							Thermometer.PolylineType = shapeType;
							Barometer.PolylineType = shapeType;
							break;
					}

			// Register library
			registrar.RegisterLibrary(namespaceName, preferredRepositoryVersion);

			registrar.RegisterShapeType(new ShapeType("Календарь", namespaceName, resourceString,
				Calendar.CreateInstance, Calendar.GetPropertyDefinitions));

			registrar.RegisterShapeType(new ShapeType("Часы", namespaceName, resourceString,
				Watch.CreateInstance, Watch.GetPropertyDefinitions));

			registrar.RegisterShapeType(new ShapeType("Флюгер", namespaceName, resourceString,
				Windvane.CreateInstance, Windvane.GetPropertyDefinitions));

			registrar.RegisterShapeType(new ShapeType("Гигрометр", namespaceName, resourceString,
				Hygrometer.CreateInstance, Hygrometer.GetPropertyDefinitions));

			registrar.RegisterShapeType(new ShapeType("Термометр", namespaceName, resourceString,
				Thermometer.CreateInstance, Thermometer.GetPropertyDefinitions));

			registrar.RegisterShapeType(new ShapeType("Барометр", namespaceName, resourceString,
				Barometer.CreateInstance, Barometer.GetPropertyDefinitions));
		}


		#region Fields

		private const string namespaceName = "FhisServices";
		private const string resourceString = "Служебные элементы";	//"ФГиИС";

		private const int preferredRepositoryVersion = 5;

		#endregion
	}
}
