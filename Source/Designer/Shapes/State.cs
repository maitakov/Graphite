using Dataweb.NShape.Advanced;
using Dataweb.NShape.WinFormsUI;

namespace Dataweb.NShape.Designer.Shapes
{	public class State
	{	static internal ShapeType CircleType;
		static internal ShapeType PropType;

		private StatePropertyCollection dynamicProperties;
		private StatePropertyCollection staticProperties;

		public StatePropertyCollection DynamicProperties { get{ return dynamicProperties; } }
		public StatePropertyCollection StaticProperties { get{ return staticProperties; } }

		public int X { get{ return dynamicProperties.circleOuter.X; } set{ dynamicProperties.circleOuter.X = value; } }
		public int Y { get{ return dynamicProperties.circleOuter.Y; } set{ dynamicProperties.circleOuter.Y = value; } }
		public int Diameter { get{ return dynamicProperties.circleOuter.Diameter; } set{ dynamicProperties.circleOuter.Diameter = value; } }
		public string ToolTipText { get{ return dynamicProperties.circleOuter.ToolTipText; } set{ dynamicProperties.circleOuter.ToolTipText = value; } }

		public State(Project project, ShapeBase parent, int diameter)
		{	int dynaDiamO, dynaDiamI, statDiamO, statDiamI;
			Init(diameter, out dynaDiamO, out dynaDiamI, out statDiamO, out statDiamI);

			dynamicProperties = new StatePropertyCollection(project, parent, dynaDiamO, dynaDiamI);
			staticProperties = new StatePropertyCollection(project, dynamicProperties.circleOuter, statDiamO, statDiamI);
		}

		public State(Project project, Display display, int diameter)
		{	int dynaDiamO, dynaDiamI, statDiamO, statDiamI;
			Init(diameter, out dynaDiamO, out dynaDiamI, out statDiamO, out statDiamI);

			dynamicProperties = new StatePropertyCollection(project, display, dynaDiamO, dynaDiamI);
			staticProperties = new StatePropertyCollection(project, dynamicProperties.circleOuter, statDiamO, statDiamI);
		}

		private void Init(int diameter, out int dynaDiamO, out int dynaDiamI, out int statDiamO, out int statDiamI)
		{	StatePropertyCollection.CircleType = CircleType;
			StatePropertyCollection.PropType = PropType;

			dynaDiamO = diameter;		dynaDiamI = dynaDiamO - 60;
			statDiamO = dynaDiamO/2;	statDiamI = statDiamO - 60;
		}
	}
}
