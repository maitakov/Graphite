using System;
using System.Collections.Generic;

using Dataweb.NShape.Advanced;
using Dataweb.NShape.WinFormsUI;

namespace Dataweb.NShape.Designer.Shapes
{	public class ActionCollection
	{	static internal ShapeType CircleType;
		static internal ShapeType ActionType;

		internal CircleBase circleOuter = null;
		internal Dictionary<int, RectangleBase> acts = new Dictionary<int, RectangleBase>();

		public Dictionary<int, RectangleBase> Acts { get{ return acts; } }
		public int Count { get{ return acts.Count; } }

		public ActionCollection(Project project, ShapeBase parent, int diameter)
		{	circleOuter = (CircleBase)CircleType.CreateInstance();
			circleOuter.IsToolbarHidden = true;
			//circleOuter.FillStyle = project.Design.FillStyles.White;
			circleOuter.Diameter = diameter;
			parent.Children.Add(circleOuter);
		}

		public ActionCollection(Project project, Display display, int diameter)
		{	circleOuter = (CircleBase)CircleType.CreateInstance();
			circleOuter.IsToolbarHidden = true;
			//circleOuter.FillStyle = project.Design.FillStyles.White;
			circleOuter.Diameter = diameter;
			display.InsertShape(circleOuter);
		}

		public RectangleBase AddAction(int id)
		{	RectangleBase act = (RectangleBase)ActionType.CreateInstance();
			act.IsToolbarHidden = true;
			acts.Add(id, act);
			circleOuter.Children.Add(act);
		//	ChangePropCount();
			return act;
		}

		public void RemoveAction(int id)
		{	RectangleBase act = acts[id];
			circleOuter.Children.Remove(act);
			acts.Remove(id);
		//	ChangePropCount();
		}

/*
		private void ChangePropCount()
		{	RectangleBase act;
			double a, da, da2;
			int rO, rI, x2, y2, cx = circleOuter.X, cy = circleOuter.Y;
			a = 0d;
			da2 = Math.PI/Count;
			da = 2d*da2;
			rO = circleOuter.Diameter/2;
			rI = circleIntegralProps.Diameter/2;
			x2 = (int)Math.Round(cx - rI*Math.Sin(da2));
			y2 = (int)Math.Round(cy - rI*Math.Cos(da2));
			//		1
			//		5
			//	2	3	4
			for(int i = 0, n = Count; i < n; a += da, ++i)
			{	act = Acts[i];

				act.MoveControlPointTo(1, (int)Math.Round(cx + rO*Math.Sin(a)), (int)Math.Round(cy - rO*Math.Cos(a)), ResizeModifiers.None);
				act.MoveControlPointTo(2, x2, y2, ResizeModifiers.None);
				act.MoveControlPointTo(3, x2 = (int)Math.Round(cx + rI*Math.Sin(a+da2)), y2 = (int)Math.Round(cy - rI*Math.Cos(a+da2)), ResizeModifiers.None);
			}
		}
*/
	}
}
