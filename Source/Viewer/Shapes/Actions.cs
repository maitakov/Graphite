using Dataweb.NShape.Advanced;
using Dataweb.NShape.WinFormsUI;

namespace Dataweb.NShape.Viewer.Shapes
{	public class Actions
	{	static internal ShapeType CircleType;
		static internal ShapeType ActionType;

		internal ActionCollection actionCollection;

		public ActionCollection Acts { get{ return actionCollection; } }

		public int X { get{ return actionCollection.circleOuter.X; } set{ actionCollection.circleOuter.X = value; } }
		public int Y { get{ return actionCollection.circleOuter.Y; } set{ actionCollection.circleOuter.Y = value; } }
		public int Diameter { get{ return actionCollection.circleOuter.Diameter; } set{ actionCollection.circleOuter.Diameter = value; } }
		public string ToolTipText { get{ return actionCollection.circleOuter.ToolTipText; } set{ actionCollection.circleOuter.ToolTipText = value; } }

		public Actions(Project project, ShapeBase parent, int diameter)
		{	ActionCollection.CircleType = CircleType;
			ActionCollection.ActionType = ActionType;
			actionCollection = new ActionCollection(project, parent, diameter);
		}

		public Actions(Project project, Display display, int diameter)
		{	ActionCollection.CircleType = CircleType;
			ActionCollection.ActionType = ActionType;
			actionCollection = new ActionCollection(project, display, diameter);
		}
	}
}
